using AutoMapper;
using Common.DTO.Auth;
using Common.DTO.Profile;
using Exceptions.ExceptionTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Text.RegularExpressions;
using UserService.Common.DTO.Auth;
using UserService.Common.DTO.Profile;
using UserService.Common.Interface;
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
        private readonly SignInManager<User> _signInManager;

        public AuthService(
            IMapper mapper,
            ITokenService tokenService,
            UserManager<User> userManager,
            RedisRepository redisRepository,
            SignInManager<User> signInManager
        )
        {
            _mapper = mapper;
            _tokenService = tokenService;
            _userManager = userManager;
            _redisRepository = redisRepository;
            _signInManager = signInManager;
        }

        public async Task ChangePassword(PasswordChangeRequestDTO passwordData, Guid userId)
        {
            if (passwordData.Password == passwordData.ConfirmPassword)
            {
                

                var user = await _userManager.FindByIdAsync(userId.ToString());

                if (user == null)
                {
                    throw new NotFoundException("Такого пользователя не существует");
                }

                var result = await _signInManager.CheckPasswordSignInAsync(user, passwordData.CurrentPassword, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var changeResult = await _userManager.ChangePasswordAsync(user, passwordData.CurrentPassword, passwordData.Password);
                    if (!changeResult.Succeeded)
                    {
                        throw new BadRequestException(string.Join(", ", changeResult.Errors.Select(x => x.Description)));
                    }
                }
                else
                {
                    throw new BadRequestException("Неверный текущий пароль");
                }


                
            }
            else
            {
                throw new BadRequestException("Пароли не совпадают");
            }
        }

        public async Task<AuthResponseDTO> Login(LoginRequestDTO loginData)
        {

            var user = await _userManager.FindByEmailAsync(loginData.Email);

        

                var result = await _signInManager.CheckPasswordSignInAsync(user, loginData.Password, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    var accessToken = await _tokenService.GenerateAccessToken(user.Id);
                    var refreshToken = await _tokenService.GenerateRefreshToken(user.Id);

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

            ValidateBirthDate(registrationData.BirthDate);
            ValidatePhoneNumber(registrationData.PhoneNumber);

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

        private void ValidatePhoneNumber(string phoneNumber)
        {
            string regexPattern = @"^((8|\+7)[\- ]?)?(\(?\d{3}\)?[\- ]?)?[\d\- ]{7,10}$";

            if (!Regex.IsMatch(phoneNumber, regexPattern))
            {
                throw new BadRequestException("Номер телефона не соответствует маске +7(8) 000 000 00 00");
            }
        }
        private void ValidateBirthDate(DateTime birthDate)
        {
            var minCheck = birthDate > DateTime.UtcNow.AddYears(-10);

            var maxCheck = birthDate < DateTime.UtcNow.AddYears(-95);

            if (minCheck)
            {
                throw new BadRequestException("Возраст должен быть минимум 10 лет");
            }
            if (maxCheck)
            {
                throw new BadRequestException("Возраст не должен быть больше 95 лет");
            }
        }


    }
}
