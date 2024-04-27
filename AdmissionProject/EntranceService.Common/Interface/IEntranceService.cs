using EntranceService.Common.DTO;
using EntranceService.DAL.Enum;

namespace EntranceService.Common.Interface
{
    public interface IEntranceService
    {
        public Task DeleteProgram(DeleteProgramDTO deleteProgramDTO);
        public Task ChangeProgramPriority(int programPriority, Guid programId, Guid applicationId);
        public Task AddProgramsToApplication(List<ProgramDTO> programsDTO, Guid aplicationId);
        public Task ChangeApplicationStatus(ApplicationStatus status, Guid applicationId);
        public Task GetApplications();
        public Task CreateApplication(Guid userId, string token, CreateApplicationDTO applicationDTO);
        
    }
}
