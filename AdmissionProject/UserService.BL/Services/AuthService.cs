using AutoMapper;
using Exceptions.ExceptionTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using UserService.Common.DTO.Auth;
using UserService.Common.DTO.Profile;
using UserService.Common.Interfaces;
using UserService.DAL.Entity;
using UserService.DAL.Repository;

namespace UserService.BL.Services
{
    public class AuthService : IAuthService
    {
        private readonly IMapper _mapper;
        private readonly ITokenService _tokenService;
        private readonly UserManager<User> _userManager;
        private readonly RedisRepository _redisRepository;

        public AuthService(
            IMapper mapper,
            ITokenService tokenService,
            UserManager<User> userManager,
            RedisRepository redisRepository
        )
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _userManager = userManager;
            _redisRepository = redisRepository;
        }

        public async Task ChangePassword(PasswordChangeRequestDTO passwordData, string token)
        {
            var userId = _tokenService.GetUserIdFromToken(token);

            if (passwordData.Password == passwordData.ConfrimPassword)
            {
                var user = await _userManager.FindByIdAsync(userId.ToString());
                if (user == null)
                {
                    throw new NotFoundException("Такого пользователя не существует");
                }
                await _userManager.ChangePasswordAsync(user, user.PasswordHash, passwordData.Password);
            }
            else
            {
                throw new BadRequestException("Пароли не совпадают");
            }
        }

        public async Task<AuthResponseDTO> Login(LoginRequestDTO loginData)
        {

            Console.WriteLine(loginData.Email);

            var user = await _userManager.FindByEmailAsync(loginData.Email);

            if (user != null)
            {
                var accessToken = _tokenService.GenerateAccessToken(user.Id);
                var refreshToken = _tokenService.GenerateRefreshToken(user.Id);

                Console.WriteLine(user.Id);
                await _tokenService.SaveRefreshTokenAsync(refreshToken.Token, user.Id);

                var tokenPair = new AuthResponseDTO
                {
                    AccessToken = accessToken,
                    RefreshToken = refreshToken.Token,
                };

                return tokenPair;
            }
            else
            {
                throw new BadRequestException("Неверный логин или пароль");
            }
        }

        public async Task Logout(string accessToken)
        {
            var userId = _tokenService.GetUserIdFromToken(accessToken);

            var user = await _userManager.Users.FirstOrDefaultAsync(user => user.Id == userId);
            if (user == null)
            {
                throw new NotFoundException("Такого пользователя не существует");
            }

            user.RefreshToken = null;
            await _redisRepository.AddTokenBlackList(accessToken);
        }

        public async Task<AuthResponseDTO> Register(RegistrationRequestDTO registrationData)
        {
            var newUser = _mapper.Map<User>(registrationData);
            newUser.UserName = newUser.Email;
            newUser.SecurityStamp = Guid.NewGuid().ToString();
            var creatingResult = await _userManager.CreateAsync(newUser, registrationData.Password);

            if (creatingResult.Succeeded)
            {
                await _userManager.AddToRoleAsync(newUser, RoleNames.User);
                var loginData = new LoginRequestDTO
                {
                    Email = registrationData.Email,
                    Password = registrationData.Password,
                };

                var tokenInfo = await Login(loginData);

                return tokenInfo;
            }
            else
            {
                var errors = creatingResult.Errors.Select(error => error.Description);
                var errorMessage = string.Join(", ", errors);
                throw new BadRequestException($"Ошибка при регистрации: {errorMessage}");
            }
        }
    }
}
