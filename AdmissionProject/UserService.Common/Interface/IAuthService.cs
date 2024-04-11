using UserService.Common.DTO.Auth;
using UserService.Common.DTO.Profile;

namespace UserService.Common.Interfaces
{
    public interface IAuthService
    {
        public Task<AuthResponseDTO> Login(LoginRequestDTO loginData);
        public Task<AuthResponseDTO> Register(RegistrationRequestDTO registrationData);
        public Task ChangePassword(PasswordChangeRequestDTO passwordData, string token);
        public Task Logout(string accessToken);
    }
}
