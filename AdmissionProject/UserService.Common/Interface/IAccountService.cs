using Common.DTO.Profile;
using Common.DTO.User;
using UserService.Common.DTO.Profile;

namespace UserService.Common.Interface
{
    public interface IAccountService
    { 
        public Task<ProfileResponseDTO> GetProfile(Guid userId);
        public Task ChangeProfileInfo(ChangeProfileRequestDTO newProfileInfo, Guid userId);
        public Task<UserRoleResponseDTO> GetMyRoles(string token);
        public Task GiveRole(UserRoleActionDTO roleRequesData);
        public Task RemoveRole(DeleteUserRoleDTO userInfo);
        public Task<GetAllUsersDTO> GetAllUsers(UsersFilterDTO filters);
    }
}
