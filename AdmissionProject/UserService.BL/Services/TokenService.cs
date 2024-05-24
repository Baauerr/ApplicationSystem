using Common.DTO.Auth;
using Exceptions.ExceptionTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using UserService.BL.Configuration;
using UserService.Common.Interface;
using UserService.DAL;
using UserService.DAL.Entity;

namespace UserService.BL.Services
{
    public class TokenService : ITokenService
    {
        private readonly TokenProps _tokenInfo;
        private readonly UserManager<User> _userManager;
        private readonly IConfiguration _configuration;
        public TokenService(TokenProps tokenInfo, UserDbContext userDb, IConfiguration configuration, UserManager<User> userManager)
        {
            _tokenInfo = tokenInfo;
            _userManager = userManager; 
            _configuration = configuration;

        }
        private async Task<string> GenerateToken(Guid? id, double expirationTime)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            var user =  await _userManager.FindByIdAsync(id.ToString());

            var roles = await _userManager.GetRolesAsync(user);

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                NotBefore = DateTime.UtcNow,
                Expires = DateTime.UtcNow.AddMinutes(expirationTime),
                SigningCredentials = new SigningCredentials(_tokenInfo.TokenKey, SecurityAlgorithms.HmacSha256Signature),
                Subject = new ClaimsIdentity(new Claim[]
                {
                new Claim(ClaimTypes.NameIdentifier, id.ToString())
                }.Concat(roles.Select(role => new Claim(ClaimTypes.Role, role)))),
                //ISSUER, AUDIENCE
            };

            var token = tokenHandler.CreateToken(tokenDescriptor);

            return tokenHandler.WriteToken(token);
        }
        public async Task<string> GenerateAccessToken(Guid? id)
        {
            return await GenerateToken(id, _tokenInfo.AccessTokenExpiration.TotalMinutes);
        }
        public async Task<RefreshTokenDTO> GenerateRefreshToken(Guid id)
        {
            var Token = await GenerateToken(id, _tokenInfo.RefreshTokenExpiration.TotalDays);
            var expiration = DateTime.UtcNow.AddMinutes(_tokenInfo.RefreshTokenExpiration.TotalMinutes);

            var refreshToken = new RefreshTokenDTO
            {
                Token = Token,
                ExpireTime = expiration,
            };

            return refreshToken;
        }
        public async Task SaveRefreshTokenAsync(string refreshToken, Guid userId)
        {
            var expiration = DateTime.UtcNow.AddMinutes(_tokenInfo.RefreshTokenExpiration.TotalMinutes);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpireTime = expiration;

            await _userManager.UpdateAsync(user);
        }
        public Guid GetUserIdFromToken(string token)
        {

            var key = _configuration.GetValue<string>("ApiSettings:SecretKey");
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));

            var validationParams = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = false,
                IssuerSigningKey = securityKey,
                ValidateAudience = false, 
                ValidateIssuer = false,
                ValidateLifetime = false,
            };

            var handler = new JwtSecurityTokenHandler();
            var claimsPrincipal = handler.ValidateToken(token, validationParams, out var validatedToken);

            if (claimsPrincipal == null)
            {
                throw new BadRequestException("В токене нет claims");
            }
            var userId = claimsPrincipal.FindFirst(ClaimTypes.NameIdentifier);

            var userGuid = Guid.Parse(userId.Value);
            if (userGuid == Guid.Empty)
            {
                throw new NotFoundException("Такого пользователя нет");
            }
            return userGuid;
        }

        public async Task<RefreshedToken> Refresh(string token)
        {
            var userId = GetUserIdFromToken(token);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
            {
                throw new NotFoundException("Такой пользователь не найден");
            }
            if (user.RefreshTokenExpireTime < DateTime.UtcNow)
            {
                user.RefreshToken = null;
                throw new BadRequestException("Refresh токен просрочен");
            }
            if (user.RefreshToken != token)
            {
                throw new BadRequestException("Refresh токен не принадлежит пользователю");
            }
            if (user.RefreshToken == null)
            {
                throw new UnauthorizedAccessException();
            }
            var newToken = new RefreshedToken
            {
                AccessToken = await GenerateAccessToken(userId)
            };

            return newToken;
        }
    }
}

