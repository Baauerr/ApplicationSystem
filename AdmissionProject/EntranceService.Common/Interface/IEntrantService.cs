using Common.DTO.Entrance;
using EntranceService.Common.DTO;

namespace EntranceService.Common.Interface
{
    public interface IEntrantService
    {
        public Task<GetApplicationDTO> GetApplicationInfo(Guid userId);
        public Task EditApplicationsInfo(EditApplicationDTO newApplicationInfo, Guid userId);
        public Task SyncUserDataInApplication(UpdateUserDataDTO userData);
    }
}
