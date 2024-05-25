using Common.DTO.Auth;
using Common.DTO.Dictionary;
using Common.DTO.Entrance;
using Common.DTO.Profile;
using Common.DTO.Document;

namespace AdminPanel.BL.Serivces.Interface
{
    public interface IQueueSender
    {
        public Task<AuthResponseDTO> Login(LoginRequestDTO loginCreds);
        public Task<ProfileResponseDTO> GetProfile(Guid userId);
        public Task SendMessage<T>(T message, string topik);
        public Task<ApplicationsResponseDTO> GetApplications(ApplicationFiltersDTO applicationFilters);
        public Task<PassportFormDTO> GetPassportForm(Guid userId);
        public Task<GetEducationDocumentFormDTO> GetEducationDocumentForm(Guid userId);
        public Task<ManagersListDTO> GetAllManagers();
        public Task<FacultiesResponseDTO> GetAllFaculties();
        public Task<ProgramResponseDTO> GetAllPrograms();
        public Task<AllImportHistoryDTO> GetImportHistory();
        public Task<EducationLevelResponseDTO> GetAllEducationLevels();
        public Task<GetApplicationPrograms> GetApplicationPrograms(Guid userId);
    }
}
