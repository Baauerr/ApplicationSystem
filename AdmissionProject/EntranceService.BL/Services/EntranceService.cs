using EntranceService.Common.Interface;
using EntranceService.Common.DTO;
using EntranceService.DAL;
using AutoMapper;
using EntranceService.DAL.Entity;
using EntranceService.DAL.Enum;
using Exceptions.ExceptionTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;
using Common.DTO.User;
using Common.Enum;
using NotificationService.DTO;
using EasyNetQ;
using DocumentService.Common.DTO;
using Common.DTO.Profile;
using Common.DTO.Dictionary;

namespace EntranceService.BL.Services
{
    public class EntrancingService : IEntranceService
    {
        private readonly EntranceDbContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly QueueSender _queueSender;
        private readonly IBus _bus;


        public EntrancingService(
            EntranceDbContext entranceDbContext,
            IMapper mapper,
            IConfiguration configuration,
            QueueSender queueSender
            )
        {
            _db = entranceDbContext;
            _mapper = mapper;
            _configuration = configuration;
            _queueSender = queueSender;
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        public async Task AddProgramsToApplication(List<ProgramDTO> programsDTO, Guid aplicationId)
        {

            var maxProgramsCount = _configuration.GetValue<int>("Programs:MaxProgramsCount");

            var applicationPrograms = _db.ApplicationsPrograms
                .Where(application => application.ApplicationId == aplicationId);


            var applicationProgramsCount = applicationPrograms.Count();

            var newProgramsCount = programsDTO.Count();

            if (newProgramsCount + applicationProgramsCount > maxProgramsCount) {
                throw new BadRequestException($"Число программ в одном заявлении не может быть больше {maxProgramsCount}");
            }

            var hasDublicatePriority = programsDTO.GroupBy(x => x.ProgramPriority)
                .Where(program => program.Count() > 1)
                .Select(program => program.Key)
                .Any();

            if (applicationPrograms != null && applicationPrograms.Any())
            {
               var hasDublicateProgram = applicationPrograms
                .Any(ap => programsDTO.Any(program => program.ProgramId == ap.ProgramId));
            }
            
            var maxPriority = programsDTO.OrderByDescending(program => program.ProgramPriority).FirstOrDefault();

            if (maxPriority.ProgramPriority > (newProgramsCount + applicationProgramsCount) || maxPriority.ProgramPriority < 1)
            {
                throw new BadRequestException("Приоритет программы не может быть больше количества программ в заявке и не может быть меньше 1");
            }

            if (hasDublicatePriority)
            {
                throw new BadRequestException("Нельзя добавить программы с одинаковыми приоритетами");
            }

            List<ApplicationPrograms> newPrograms = new List<ApplicationPrograms>();

            programsDTO.ForEach(program =>
            {
                var applicationPrograms = new ApplicationPrograms
                {
                    ApplicationId = aplicationId,
                    Priority = program.ProgramPriority,
                    ProgramId = program.ProgramId
                };

                newPrograms.Add(applicationPrograms);
            });

            await _db.ApplicationsPrograms.AddRangeAsync(newPrograms);
            await _db.SaveChangesAsync();
        }

        private async Task ValidatePrograms(List<ProgramDTO> programsDTO, Guid userId)
        {
            var availablePrograms = await _queueSender.GetAllPrograms(userId);

            availablePrograms.programs.RemoveAll(program => !programsDTO.Any(newProgram => newProgram.ProgramId == program.Id));

            var firstEducationLevelId = availablePrograms.programs.First().EducationLevel.Id;

            await CheckEducationLevel(userId, firstEducationLevelId);

            bool allSameId = availablePrograms.programs.All(program => program.EducationLevel.Id == firstEducationLevelId);

            if (!allSameId)
            {
                throw new BadRequestException("Нельзя добавить в одно заявление программы с разным уровнем образования");
            }

            var isProgramsExist = availablePrograms.programs.Count() == programsDTO.Count();

            if (!isProgramsExist)
            {
                throw new NotFoundException("В списке есть несуществующая программа");
            }

            var hasBublicatePrograms = programsDTO
                .GroupBy(x => x)
                .Where(program => program.Count() > 1)
                .Select(program => program.Key)
                .ToList();

            if (hasBublicatePrograms.Any())
            {
                throw new BadRequestException("Группы не могут повторяться в заявлении");
            }
        }

        public async Task ChangeApplicationStatus(ApplicationStatus status, Guid applicationId)
        {
            var application = await _db.Applications
                .Where(ap => ap.Id == applicationId)
                .FirstOrDefaultAsync();

            if (application == null)
            {
                throw new NotFoundException("Заявления с таким id не существует");
            }

            application.ApplicationStatus = status;
            application.LastChangeDate = DateTime.UtcNow.ToUniversalTime();

            _db.Applications.Update(application);

            await _db.SaveChangesAsync();
        }

        public async Task ChangeProgramPriority(ChangeProgramPriorityDTO changeProgramPriorityDTO, Guid userId)
        {

            var application = _db.Applications.FirstOrDefault(ap => ap.OwnerId == userId);

            if (application == null)
            {
                throw new NotFoundException("Заявления с таким id не существует"); 
            }

            if (application.OwnerId != userId || application.ManagerId != userId)
            {
                throw new ForbiddenException("Нет доступа к этой заявке");
            }

            var programs = await _db.ApplicationsPrograms
                .Where(ap => ap.ApplicationId == changeProgramPriorityDTO.ApplicationId)
                .ToListAsync();

            var programToEdit = programs.FirstOrDefault(p => p.ProgramId == changeProgramPriorityDTO.ProgramId);

            

            if (programToEdit == null)
            {
                throw new NotFoundException("В этой заявке нет такой программы");
            }

            _db.ApplicationsPrograms.Update(programToEdit);

            var maxProgramsCount = _configuration.GetValue<int>("Programs:MaxProgramsCount");

            if (programs.Count() < changeProgramPriorityDTO.Priority && changeProgramPriorityDTO.Priority < 1)
            {
                throw new BadRequestException("Приоритет не может быть больше количества программ и меньше 1");
            }

            programToEdit.Priority = changeProgramPriorityDTO.Priority;

            foreach (var program in programs)
            {
                if (program.Priority >= changeProgramPriorityDTO.Priority && program.ProgramId != changeProgramPriorityDTO.ProgramId)
                {
                    program.Priority = program.Priority + 1;
                }
            }

            _db.ApplicationsPrograms.UpdateRange(programs);

            await _db.SaveChangesAsync();

        }

        public async Task CreateApplication(Guid userId, string token, CreateApplicationDTO applicationDTO)
        {
            await ValidatePrograms(applicationDTO.Programs, userId);

            await CheckPasportExist(userId);

            var userInfo = await GetUserInfo(userId);

            var application = new Application
            {
                Citizenship = applicationDTO.Citizenship,
                Id = Guid.NewGuid(),
                OwnerId = userId,
                LastChangeDate = DateTime.UtcNow.ToUniversalTime(),
                ApplicationStatus = ApplicationStatus.Pending,
                OwnerName = userInfo.FullName,
            };
            
            await AddProgramsToApplication(applicationDTO.Programs, application.Id);

            application.LastChangeDate = DateTime.UtcNow.ToUniversalTime();

            await _db.Applications.AddAsync(application);
            await _db.SaveChangesAsync();
            await GiveEntrantRole(userId);
        }
        private async Task<ProfileResponseDTO> GetUserInfo(Guid userId)
        {
            var userInfo = await _bus.Rpc.RequestAsync<UserIdDTO, ProfileResponseDTO>(new UserIdDTO { UserId = userId });

            return userInfo;
        }

        public async Task DeleteProgram(DeleteProgramDTO deleteProgramDTO, Guid userId)
        {
            var application = await _db.Applications.FirstOrDefaultAsync(ap => ap.Id == deleteProgramDTO.ApplicationId);

            if (application == null)
            {
                throw new NotFoundException("Такой заявки не существует");
            }

                if (application.OwnerId != userId || application.ManagerId == userId)
                {
                    throw new ForbiddenException("Пользователь не имеет права редактировать это заявление");
                }
            
            

            var applicationProgram = await _db.ApplicationsPrograms.FirstOrDefaultAsync(
                ap => ap.ProgramId == deleteProgramDTO.ProgramId
                && ap.ApplicationId == deleteProgramDTO.ApplicationId);
            
            if (applicationProgram == null)
            {
                throw new NotFoundException("В данном заявлении нет такой программы или заявление не существует");
            }

            var applicationPrograms = await _db.ApplicationsPrograms
                .Where(ap => ap.ProgramId == deleteProgramDTO.ProgramId)
                .ToListAsync();

            for (int i = applicationProgram.Priority; i < applicationPrograms.Count(); i++)
            {
                applicationPrograms[i].Priority = applicationPrograms[i].Priority--;
            }

            _db.ApplicationsPrograms.UpdateRange(applicationPrograms);

            _db.ApplicationsPrograms.Remove(applicationProgram);
            await _db.SaveChangesAsync();
        }

        private async Task CheckPasportExist(Guid userId)
        {
                var passportInfo = await _queueSender.GetUserPassport(userId);

                if (passportInfo == null)
                {
                    throw new BadRequestException("Невозможно создать заявление, если не загружен паспорт");
                }
        }

        private async Task CheckEducationLevel(Guid userId, string educationLevelId)
        {

            //TODO ПРОВЕРИТЬ, ЧТО УРОВЕНЬ ОБРАЗОВАНИЯ ПРОГРММЫ = NEXTEDULEVELID от документа об образовании
            

                throw new BadRequestException("У пользователя нет подходящего документа об образовании");
                    
        }


        public async Task GetApplications()
        {
             

        }

        private async Task GiveEntrantRole(Guid userId)
        {

            var message = new SetRoleRequestDTO
            {
                RecipientId = userId.ToString(),
                Role = Roles.ENTRANT
            };

        //    await _queueSender.SendMessage(message);      
        }
        public async Task SetManager(AddManagerDTO addManagerDTO)
        {
            var application = await _db.Applications.FirstOrDefaultAsync(ap => ap.Id == addManagerDTO.ApplicationId);

            if (application == null){
                throw new NotFoundException("Такой заявки не существует");
            }
            if (application.ManagerId == Guid.Empty)
            {
                application.ManagerId = addManagerDTO.ManagerId;
                application.LastChangeDate = DateTime.UtcNow.ToUniversalTime();
                _db.Applications.Update(application);
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new BadRequestException("У этой заявки уже есть менеджер");
            }
        }
        private async Task SendNotification(string recipient, string managerFullName)
        {
            var message = new MailStructure
            {
                Recipient = recipient,
                Body =  $"На ваше заявление был назначен менеджер {managerFullName}",
                Subject = "Назначение менеджера"
            };

            await _queueSender.SendMessage(message);
        }
    }
    
}
