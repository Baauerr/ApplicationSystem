using EntranceService.Common.Interface;
using EntranceService.Common.DTO;
using EntranceService.DAL;
using AutoMapper;
using EntranceService.DAL.Entity;
using EntranceService.DAL.Enum;
using Exceptions.ExceptionTypes;
using Microsoft.Extensions.Configuration;
using Microsoft.EntityFrameworkCore;

namespace EntranceService.BL.Services
{
    public class EntrancingService : IEntranceService
    {
        private readonly EntranceDbContext _db;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRequestService _requestService;
        private readonly IDocumentService _documentService;

        public EntrancingService(
            EntranceDbContext entranceDbContext,
            IMapper mapper,
            IConfiguration configuration,
            IRequestService requestService,
            IDocumentService documentService
            )
        {
            _db = entranceDbContext;
            _mapper = mapper;
            _configuration = configuration;
            _requestService = requestService;
            _documentService = documentService;
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
            var availablePrograms = await _requestService.GetPrograms();

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

            await _db.SaveChangesAsync();
        }

        public Task ChangeProgramPriority(int programPriority, Guid programId, Guid applicationId)
        {
            throw new NotImplementedException();

        }

        public async Task CreateApplication(Guid userId, string token, CreateApplicationDTO applicationDTO)
        {
            await ValidatePrograms(applicationDTO.Programs, userId);

            await CheckPasportExist(userId);

            var application = new Application
            {
                Citizenship = applicationDTO.Citizenship,
                Id = Guid.NewGuid(),
                OwnerId = userId,
                LastChangeDate = DateTime.UtcNow.ToUniversalTime(),
                ApplicationStatus = ApplicationStatus.Pending,
            };
            
            await AddProgramsToApplication(applicationDTO.Programs, application.Id);

            application.LastChangeDate = DateTime.UtcNow.ToUniversalTime();

            await _db.Applications.AddAsync(application);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteProgram(DeleteProgramDTO deleteProgramDTO)
        {
            var applicationProgram = await _db.ApplicationsPrograms.FirstOrDefaultAsync(
                ap => ap.ProgramId == deleteProgramDTO.ProgramId && ap.ApplicationId == deleteProgramDTO.ApplicationId);
            
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
            var passportExists = await _documentService.GetPassportInfo(userId);

            if (passportExists == null)
            {
                throw new BadRequestException("Невозможно создать заявление, если не загружен паспорт");
            }
        }

        private async Task CheckEducationLevel(Guid userId, string educationLevelId)
        {
            var educationDocuments = await _documentService.GetEducationDocumentsInfo(userId);

            var educationDocumentExist = educationDocuments.Any(ed => ed.EducationLevelId == educationLevelId);

            if (!educationDocumentExist) {
                throw new BadRequestException("У пользователя нет подходящего документа об образовании");
            }           
        }


        public async Task GetApplications()
        {
             
        }
        
    }
}
