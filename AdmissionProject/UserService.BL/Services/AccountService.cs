using AutoMapper;
using Exceptions.ExceptionTypes;
using Microsoft.AspNetCore.Identity;
using System.Reflection.Metadata.Ecma335;
using UserService.Common.DTO.Profile;
using UserService.Common.Interfaces;
using UserService.DAL.Entity;

namespace UserService.BL.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;

        public AccountService(UserManager<User> userManager, ITokenService tokenService, IMapper mapper) {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
        }
        public async Task ChangeProfileInfo(ChangeProfileRequestDTO newProfileInfo, string token)
        {
            var userId = _tokenService.GetUserIdFromToken(token);

            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null)
                throw new NotFoundException("Такого пользователя не существует");


            if (newProfileInfo.FullName == null)
                throw new BadRequestException("Имя не должно быть пустым");
            if (newProfileInfo.Email == null)
                throw new BadRequestException("Email не может быть пустым");

            user.FullName = newProfileInfo.FullName;
            user.PhoneNumber = newProfileInfo.PhoneNumber;
            user.Email = newProfileInfo.Email;
            user.UserName = newProfileInfo.Email;
            user.BirthDate = newProfileInfo.BirthDate;
            user.Citizenship = newProfileInfo.Citizenship;

            user.SecurityStamp = Guid.NewGuid().ToString();

            var result = await _userManager.UpdateAsync(user);
            if (result.Succeeded)
            {
                return;
            }
            else
            {
                throw new BadRequestException($"{result}"); 
            }
        }

        public async Task<ProfileResponseDTO> GetProfile(string token)
        {
            var userId = _tokenService.GetUserIdFromToken(token);
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var userProfile = _mapper.Map<ProfileResponseDTO>(user);
            return userProfile;
        }
        public async Task<UserRoleResponseDTO> GetMyRoles(string token)
        {
            var userId = _tokenService.GetUserIdFromToken(token);
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) throw new NotFoundException("Такого пользователя не существует");

            var roles = await _userManager.GetRolesAsync(user);

            var rolesDTO = new UserRoleResponseDTO
            {
                roles = roles
            };

            return rolesDTO;
        }

        public async Task GiveRole()
        {
            var id = "ef3ad9fe-3c3b-4526-bb52-52848d75853e";
            var role = Roles.Manager;
            var user = await _userManager.FindByIdAsync(id);
            if (user == null) throw new NotFoundException("Такого пользователя не существует");
            await _userManager.AddToRoleAsync(user, role.ToString());
        }
    }
}
