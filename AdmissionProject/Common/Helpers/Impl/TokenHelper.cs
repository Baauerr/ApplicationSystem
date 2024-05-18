using Exceptions.ExceptionTypes;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace Common.Helpers.Impl
{
    public class TokenHelper: ITokenHelper
    {

        private readonly IHttpContextAccessor _httpContextAccessor;

        public TokenHelper(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string GetTokenFromHeader()
        {

            var context = _httpContextAccessor.HttpContext;

            if (context == null)
            {
                throw new InvalidOperationException("HttpContext is not available");
            }

            string token = context.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");

            return token;
        }
        public Guid GetUserIdFromToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var securityToken = tokenHandler.ReadJwtToken(token);
            var userIdClaim = securityToken.Claims.FirstOrDefault(c => c.Type == "nameid");

            if (userIdClaim != null)
            {
                return Guid.Parse(userIdClaim.Value);
            }
            else
            {
                throw new NotFoundException("В токене нет id пользователя");
            }
        }
    }
}
