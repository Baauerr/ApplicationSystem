using Common.Enum;
using Exceptions.ExceptionTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Common.Helpers.Impl
{
    public class TokenHelper: ITokenHelper
    {

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IConfiguration _configuration;

        public TokenHelper(IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
        {
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
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
        public IEnumerable<string> GetUserRolesFromToken(string token)
        {

            var secretKey = _configuration.GetValue<string>("ApiSettings:SecretKey");

            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secretKey));

            var validationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = securityKey,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = false
            };

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var claimsPrincipal = handler.ValidateToken(token, validationParameters, out var validatedToken);

                if (claimsPrincipal is null || !(validatedToken is JwtSecurityToken jwtSecurityToken))
                    return Enumerable.Empty<string>();

                var roleClaims = claimsPrincipal.FindAll(ClaimTypes.Role);
                return roleClaims.Select(rc => rc.Value);
            }
            catch (Exception)
            {
                return Enumerable.Empty<string>();
            }
        }
    }
}
