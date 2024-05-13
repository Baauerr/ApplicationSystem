using Common.DTO.Dictionary;
using Common.DTO.Profile;
using EntranceService.Common.DTO;
using EntranceService.DAL.Enum;


namespace EntranceService.Common.Interface
{
    public interface IEntranceService
    {
        public Task DeleteProgram(DeleteProgramDTO deleteProgramDTO, Guid userId);
        public Task ChangeProgramPriority(ChangeProgramPriorityDTO changeProgramPriorityDTO, Guid userId);
        public Task AddProgramsToApplication(List<ProgramDTO> programsDTO, Guid aplicationId, Guid userId);
        public Task ChangeApplicationStatus(ApplicationStatus status, Guid applicationId);
        public Task<ApplicationsResponseDTO> GetApplications(
            string? entrantName,
            Guid? programsGuid,
            List<string>? faculties,
            ApplicationStatus? status,
            bool? hasManager,
            Guid? managerId,
            SortingTypes? sortingTypes,
            int page,
            int pageSize
            );
        public Task CreateApplication(Guid userId, string token, CreateApplicationDTO applicationDTO);
        public Task SetManager(AddManagerDTO addManagerDTO);
        public Task RefuseApplication(RefuseApplicationDTO refuseApplicationDTO);
        public Task RemoveManager(Guid managerGuid);
    }
}
