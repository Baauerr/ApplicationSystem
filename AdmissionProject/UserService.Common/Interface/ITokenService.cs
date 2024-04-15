using UserService.Common.DTO.Auth;

namespace UserService.Common.Interfaces
{
    public interface ITokenService
    {
        public Task<string> GenerateAccessToken(Guid? id);
        public Task<RefreshTokenDTO> GenerateRefreshToken(Guid id);
        public Task SaveRefreshTokenAsync(string refreshToken, Guid userId);
        public Guid GetUserIdFromToken(string token);
        public Task<RefreshedToken> Refresh(string token);
    }
}
