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
using Common.Const;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Extensions.Hosting.Internal;
using Microsoft.IdentityModel.Tokens;
using System.Drawing;
using System.Linq;

namespace EntranceService.BL.Services
{
    public class EntrancingService : IEntranceService
    {
        private readonly EntranceDbContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly QueueSender _queueSender;


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
        }

        public async Task AddProgramsToApplication(List<ProgramDTO> programsDTO, Guid aplicationId, ProgramResponseDTO availablePrograms)
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

                var allProgramInfo = availablePrograms.programs.FirstOrDefault(ap => ap.Id == program.ProgramId);

                var applicationPrograms = new ApplicationPrograms
                {
                    ApplicationId = aplicationId,
                    Priority = program.ProgramPriority,
                    ProgramId = program.ProgramId,
                    FacultyId = program.FacultyId,
                    ProgramName = allProgramInfo.Name,
                    FacultyName = allProgramInfo.Faculty.Name,
                    
                };

                newPrograms.Add(applicationPrograms);
            });

            await _db.ApplicationsPrograms.AddRangeAsync(newPrograms);
            await _db.SaveChangesAsync();
        }

        private async Task ValidatePrograms(List<ProgramDTO> programsDTO, Guid userId, ProgramResponseDTO availablePrograms)
        {

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

            var message = new MailStructure
            {
                Recipient = application.OwnerEmail,
                Body = $"У вашего заявления изменился статус на {status}",
                Subject = "Изменение статуса заявления"
            };

            await SendNotification(message);
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

            var availablePrograms = await _queueSender.GetAllPrograms(userId);

            await ValidatePrograms(applicationDTO.Programs, userId, availablePrograms);

            await CheckPasportExist(userId);

            var userInfo = await _queueSender.GetProfileInfo(userId);

            var application = new Application
            {
                Citizenship = applicationDTO.Citizenship,
                Id = Guid.NewGuid(),
                OwnerId = userId,
                OwnerEmail = userInfo.Email,
                LastChangeDate = DateTime.UtcNow.ToUniversalTime(),
                ApplicationStatus = ApplicationStatus.Pending,
                OwnerName = userInfo.FullName,
            };
            
            await AddProgramsToApplication(applicationDTO.Programs, application.Id, availablePrograms);

            application.LastChangeDate = DateTime.UtcNow.ToUniversalTime();

            await _db.Applications.AddAsync(application);
            await _db.SaveChangesAsync();
            await GiveEntrantRole(userId);
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
            

          //      throw new BadRequestException("У пользователя нет подходящего документа об образовании");
                    
        }


        public async Task<ApplicationsResponseDTO> GetApplications(
            string? entrantName, 
            Guid? programsGuid, 
            List<string>? faculties, 
            ApplicationStatus? status, 
            bool? hasManager, 
            Guid? managerId, 
            SortingTypes? sortingTypes,
            int page, 
            int pageSize
            )
        {

            var applications = await FilterApplications(
                entrantName, 
                programsGuid, 
                faculties, 
                status,
                hasManager,
                managerId,
                sortingTypes,
                page,
                pageSize
                ).ToListAsync();

            var applcationDTO = CreateApplicationDTO(applications, pageSize, page);

            return applcationDTO;

        }

        private IQueryable<Application> FilterApplications(
            string? entrantName,
            Guid? programGuid,
            List<string>? faculties,
            ApplicationStatus? status,
            bool? hasManager,
            Guid? managerId,
            SortingTypes? sortingTypes,
            int page,
            int pageSize
            )
        {
            var applications =  FilterByEntrantName(entrantName);
            applications =  FilterByFaculties(applications, faculties);
            applications =  FilterByPrograms(applications, programGuid);
            applications =  FilterByStatus(applications, status);
            applications =  FilterByHavingManager(applications, hasManager);
            applications =  FilterByManagerId(applications, managerId);
            applications =  FilterByPagination(applications, page, pageSize);
            applications =  SortApplications(applications, sortingTypes);

            return applications;
        }

        private IQueryable<Application> FilterByEntrantName(string? entrantName)
        {
            var query = _db.Applications.AsQueryable();

            if (!string.IsNullOrEmpty(entrantName))
            {
                query = query.Where(ap => ap.OwnerName.Contains(entrantName));
            }

            return query;
        }

        private IQueryable<Application> FilterByPrograms(
            IQueryable<Application> applications,
            Guid? programGuid
            )
        {

            var programApplicationsGuid = _db.ApplicationsPrograms
                .Where(ap => ap.ProgramId == programGuid)
                .Select(ap => ap.ApplicationId)
                .AsQueryable();

            if (programGuid != null)
            {
                applications = applications.Where(ap => programApplicationsGuid.Contains(ap.Id));
            }

            return applications;
        }

        private IQueryable<Application> FilterByFaculties(
            IQueryable<Application> applications,
            List<string>? faculties
            )
        {

            if (faculties != null && faculties.Any())
            {
                var applicationWithFaculties = _db.ApplicationsPrograms
                    .Where(ap => faculties.Contains(ap.FacultyId))
                    .Select(ap => ap.ApplicationId)
                    .AsQueryable();
                
                applications = applications.Where(ap => applicationWithFaculties.Contains(ap.Id));
            }
            return applications;

        }
        private IQueryable<Application> FilterByStatus(
            IQueryable<Application> applications,
            ApplicationStatus? status
            )
        {
            if (status != null)
            {
                applications = applications.Where(ap => ap.ApplicationStatus == status);
            }
            return applications;
        }

        private IQueryable<Application> FilterByHavingManager(
            IQueryable<Application> applications,
            bool? havingManager
            )
        {
            if (havingManager != null)
            {
                if (havingManager == true)
                {
                    applications = applications.Where(ap => ap.ManagerId != Guid.Empty);
                }
                else
                {
                    applications = applications.Where(ap => ap.ManagerId == Guid.Empty);
                }
            }

            return applications;

        }

        private Pagination GetPagination(int size, int page, int elementsCount)
        {
            var pagination = new Pagination
            {
                count = (int)Math.Ceiling((double)elementsCount / size),
                current = page,
                size = size
            };
            return pagination;
        }

        private IQueryable<Application> FilterByManagerId(
            IQueryable<Application> applications,
            Guid? managerId
            )
        {
            if (managerId != null)
            {
                applications = applications.Where(ap => ap.ManagerId == managerId);
            }

            return applications;
        }

        private IQueryable<Application> FilterByPagination(
            IQueryable<Application> applications,
            int page, 
            int pageSize
            )
        {
            if (page <= 0)
            {
                page = 1;
            }

            var count = applications.Count();

            var countOfPages = (int)Math.Ceiling((double)applications.Count() / pageSize);

            if (page <= countOfPages)
            {
                var lowerBound = page == 1 ? 0 : (page - 1) * pageSize;
                if (page < countOfPages)
                {
                    applications = applications.Skip(lowerBound).Take(pageSize);
                }
                else
                {
                    applications = applications.Skip(lowerBound).Take(applications.Count() - lowerBound);
                }
                return applications;
            }
            else
            {
                throw new BadRequestException("Такой страницы нет");
            }
        }

        private ApplicationsResponseDTO CreateApplicationDTO(
            List<Application> applications,
            int pageSize,
            int page
            )
        {
            var programsCount = applications.Count();

            List<GetApplicationDTO> applicationsDTO = new List<GetApplicationDTO>();

            foreach (var application in applications)
            {
                var applicationPrograms = _db.ApplicationsPrograms.Where(ap => ap.ApplicationId == application.Id);

                List<GetProgramDTO> programsDTO = new List<GetProgramDTO>();

                foreach (var programs in applicationPrograms)
                {
                    var programDTO = _mapper.Map<GetProgramDTO>(programs);

                    programsDTO.Add(programDTO);
                }

                var applicationDTO = _mapper.Map<GetApplicationDTO>(application);
                applicationDTO.Programs = programsDTO;

                applicationsDTO.Add(applicationDTO);
            }

            var applicationsResponseDTO = new ApplicationsResponseDTO
            {
                Applications = applicationsDTO,
                Pagination = GetPagination(pageSize, page, programsCount)
            };
            return applicationsResponseDTO;
        }

        private IQueryable<Application> SortApplications(
            IQueryable<Application> application,
            SortingTypes? sortingType
            )
        {

            switch ( sortingType )
            {
                case SortingTypes.ChangeTimeDesc:
                    application = application.OrderByDescending(d => d.LastChangeDate);
                    break;
                case SortingTypes.ChangeTimeAsc:
                    application = application.OrderBy(d => d.LastChangeDate);
                    break;
            }

            return application;
        }

        private async Task GiveEntrantRole(Guid userId)
        {

            var message = new SetRoleRequestDTO
            {
                RecipientId = userId.ToString(),
                Role = Roles.ENTRANT
            };

            await _queueSender.SendMessage(message, QueueConst.SetRoleQueue);      
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

                var message = new MailStructure
                {
                    Recipient = application.OwnerEmail,
                    Body = $"На ваше заявление был назначен менеджер {addManagerDTO.ManagerFullName}",
                    Subject = "Назначение менеджера"
                };

                await SendNotification(message);

            }
            else
            {
                throw new BadRequestException("У этой заявки уже есть менеджер");
            }
        }
        private async Task SendNotification(MailStructure message)
        {
            await _queueSender.SendMessage(message, QueueConst.NotificationQueue);
        }

        public async Task RefuseApplication(RefuseApplicationDTO refuseApplicationDTO)
        {
            var application = await _db.Applications.FirstOrDefaultAsync
                (ap => ap.Id == refuseApplicationDTO.ApplicationId);

            if (application == null)
            {
                throw new NotFoundException("Такой заявки не существует");
            }

            if (application.ManagerId != refuseApplicationDTO.ManagerId)
            {
                throw new ForbiddenException("Эта заявка не принадлежит этому менеджеру");
            }

            if (application.ManagerId != Guid.Empty)
            {
                application.ManagerId = Guid.Empty;
                application.LastChangeDate = DateTime.UtcNow.ToUniversalTime();
                _db.Applications.Update(application);
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new BadRequestException("У этой заявки нет менеджера");
            }
        }

        public async Task RemoveManager(Guid managerGuid)
        {
            var applications = _db.Applications.Where(ap => ap.ManagerId == managerGuid);

            foreach (var application in applications)
            {
                application.ManagerId = Guid.Empty;
            }
            _db.Applications.UpdateRange(applications);
            await _db.SaveChangesAsync();
        }
    }
    
}
