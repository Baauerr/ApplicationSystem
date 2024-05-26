using Common.DTO.Dictionary;
using Common.DTO.Entrance;
using Common.Enum;


namespace EntranceService.Common.Interface
{
    public interface IEntranceService
    {
        public Task DeleteProgram(DeleteProgramDTO deleteProgramDTO, Guid userId, Guid? managerId);
        public Task ChangeProgramPriority(ChangeProgramPriorityDTO changeProgramPriorityDTO, Guid userId, Guid? managerId);
        public Task AddProgramsToApplication(List<ProgramDTO> programsDTO, Guid aplicationId, Guid userId);
        public Task ChangeApplicationStatus(ApplicationStatus status, Guid applicationId);
        public Task<ApplicationsResponseDTO> GetApplications(
            string? entrantName,
            Guid? programsGuid,
            List<string>? faculties,
            ApplicationStatus? status,
            bool? hasManager,
            bool? onlyMyManaging,
            Guid? managerId,
            SortingTypes? sortingTypes,
            int page,
            int pageSize,
            Guid myId
            );
        public Task CreateApplication(Guid userId, string token, CreateApplicationDTO applicationDTO);
        public Task SetManager(TakeApplication addManagerDTO);
        public Task RefuseApplication(RefuseApplication refuseApplicationDTO);
        public Task RemoveManager(Guid managerGuid);
        public ManagersListDTO GetAllManagers();
        public Task CreateManager(ManagerDTO managerInfo);
        public Task<GetApplicationPrograms> GetApplicationPrograms(Guid userId);
        public Task<GetApplicationManagerId> GetApplicationManagerId(Guid userId);
    }

}
