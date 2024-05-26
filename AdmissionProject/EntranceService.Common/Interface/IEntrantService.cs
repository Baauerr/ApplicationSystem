using Common.DTO.Entrance;

namespace EntranceService.Common.Interface
{
    public interface IEntrantService
    {
        public Task<GetApplicationDTO> GetApplicationInfo(Guid userId);
        public Task SyncUserDataInApplication(UpdateUserDataDTO userData);
    }
}
