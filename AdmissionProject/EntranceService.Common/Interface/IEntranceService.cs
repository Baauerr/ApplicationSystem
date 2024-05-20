using Common.DTO.Dictionary;
using Common.DTO.Entrance;
using Common.Enum;


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
            string? managerName,
            SortingTypes? sortingTypes,
            int page,
            int pageSize
            );
        public Task CreateApplication(Guid userId, string token, CreateApplicationDTO applicationDTO);
        public Task SetManager(TakeApplication addManagerDTO);
        public Task RefuseApplication(RefuseApplication refuseApplicationDTO);
        public Task RemoveManager(Guid managerGuid);
    }
}
