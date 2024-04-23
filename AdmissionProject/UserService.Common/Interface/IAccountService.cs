using Common.DTO.Profile;
using UserService.Common.DTO.Profile;

namespace UserService.Common.Interfaces
{
    public interface IAccountService
    {
        public Task<ProfileResponseDTO> GetProfile(string token);
        public Task ChangeProfileInfo(ChangeProfileRequestDTO newProfileInfo, string token);
        public Task<UserRoleResponseDTO> GetMyRoles(string token);
        public Task GiveRole(SetRoleRequestDTO roleRequesData);
        public Task<List<ProfileResponseDTO>> GetManagers(string fullName);
    }
}
