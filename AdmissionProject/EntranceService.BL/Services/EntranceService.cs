using EntranceService.Common.Interface;
using EntranceService.Common.DTO;
using EntranceService.DAL;
using AutoMapper;
using EntranceService.DAL.Entity;
using EntranceService.DAL.Enum;
using Exceptions.ExceptionTypes;

namespace EntranceService.BL.Services
{
    public class EntrancingService : IEntranceService
    {
        private readonly EntranceDbContext _db;
        private readonly IMapper _mapper;

        public EntrancingService(EntranceDbContext entranceDbContext, IMapper mapper) { 
            _db = entranceDbContext;
            _mapper = mapper;
        }


        public async Task AddProgramsToApplication(List<ProgramDTO> programsDTO)
        {
            var dublicate = programsDTO.GroupBy(x => x.ProgramPriority)
                .Where(program => program.Count() > 1)
                .Select(program => program.Key); 

            if (dublicate.Any())
            {
                throw new BadRequestException("Нельзя добавить программы с одинаковыми приоритетами");
            }

            List<ApplicationPrograms> newPrograms = new List<ApplicationPrograms>();

            programsDTO.ForEach(program =>
            {
                var applicationPrograms = _mapper.Map<ApplicationPrograms>(program);
                newPrograms.Add(applicationPrograms);
            });

            await _db.ApplicationsPrograms.AddRangeAsync(newPrograms);
        }

        public Task ChangeApplicationStatus(string status, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        public Task ChangeProgramPriority(int programPriority, Guid programId, Guid applicationId)
        {
            throw new NotImplementedException();
            
        }

        public async Task CreateApplication(Guid userId, string token, CreateApplicationDTO applicationDTO)
        {
            var application = _mapper.Map<Application>(applicationDTO);

            application.OwnerId = userId;
            application.LastChangeDate = DateTime.UtcNow.ToUniversalTime();
            application.ApplicationStatus = ApplicationStatus.Pending;

            await _db.Applications.AddAsync(application);
        }


        public Task DeleteProgram(Guid programId, Guid applicationId)
        {
            throw new NotImplementedException();
        }

        private void ValidatePrograms()
        {

        }

        public Task GetApplications()
        {
            
        }
        
    }
}
