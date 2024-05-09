using Common.DTO.Profile;
using Common.DTO.User;
using UserService.Common.DTO.Profile;

namespace UserService.Common.Interfaces
{
    public interface IAccountService
    { 
        public Task<ProfileResponseDTO> GetProfile(Guid userId);
        public Task ChangeProfileInfo(ChangeProfileRequestDTO newProfileInfo, string token);
        public Task<UserRoleResponseDTO> GetMyRoles(string token);
        public Task GiveRole(SetRoleRequestDTO roleRequesData);
        public Task<List<ProfileResponseDTO>> GetManagers(string fullName);
    }
}
