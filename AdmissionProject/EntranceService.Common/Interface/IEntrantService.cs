using Common.DTO.Entrance;
using EntranceService.Common.DTO;

namespace EntranceService.Common.Interface
{
    public interface IEntrantService
    {
        public Task<GetApplicationDTO> GetApplicationInfo(Guid applicationId);
        public Task EditApplicationsInfo(EditApplicationDTO newApplicationInfo, Guid userId);
        public Task SyncNameInApplication(UpdateUserDataDTO userData);
    }
}
