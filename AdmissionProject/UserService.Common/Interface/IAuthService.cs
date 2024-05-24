using Common.DTO.Auth;
using Common.DTO.Profile;
using UserService.Common.DTO.Auth;

namespace UserService.Common.Interface
{
    public interface IAuthService
    {
        public Task<AuthResponseDTO> Login(LoginRequestDTO loginData);
        public Task<AuthResponseDTO> Register(RegistrationRequestDTO registrationData);
        public Task ChangePassword(PasswordChangeRequestDTO passwordData, Guid userId);
        public Task Logout(string accessToken);
    }
}
