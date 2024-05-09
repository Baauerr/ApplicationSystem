using AutoMapper;
using Common.Const;
using Common.DTO.Entrance;
using Common.DTO.Profile;
using Common.DTO.User;
using Common.Enum;
using DocumentService.Common.DTO;
using EasyNetQ;
using Exceptions.ExceptionTypes;
using Microsoft.AspNetCore.Connections;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using StackExchange.Redis;
using System.Text;
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
        private readonly IBus _bus;

        public AccountService(UserManager<User> userManager, ITokenService tokenService, IMapper mapper) {    
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _bus = RabbitHutch.CreateBus("host=localhost");
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

            await SyncProfileInfo(userId, newProfileInfo.FullName);

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

        private async Task SyncProfileInfo(Guid userId, string newName)
        {
            var updateInfo = new UpdateUserDataDTO
            {
                UserId = userId,
                NewUserName = newName,
            };

            await _bus.PubSub.PublishAsync(updateInfo, QueueConst.UpdateUserFullNameQueue);
        }

        public async Task<ProfileResponseDTO> GetProfile(Guid userId)
        {
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

        public async Task GiveRole(SetRoleRequestDTO roleRequesData)
        {
            var role = roleRequesData.Role;
            var user = await _userManager.FindByIdAsync(roleRequesData.RecipientId);
            if (user == null) throw new NotFoundException("Такого пользователя не существует");
            await _userManager.AddToRoleAsync(user, role.ToString());
        }
        public async Task<List<ProfileResponseDTO>> GetManagers(string fullName)
        {
            var allManagersDTO = new List<ProfileResponseDTO>();

            var managers = await _userManager.GetUsersInRoleAsync(Roles.MANAGER.ToString());
            var mainManagers = await _userManager.GetUsersInRoleAsync(Roles.MAINMANAGER.ToString());

            foreach (var manager in managers)
            {
                if (manager.FullName.Contains(fullName))
                {
                    var managerDTO = _mapper.Map<ProfileResponseDTO>(manager);
                    allManagersDTO.Add(managerDTO);
                }
            }

            foreach (var mainManager in mainManagers)
            {
                if (mainManager.FullName.Contains(fullName))
                {
                    var mainManagerDTO = _mapper.Map<ProfileResponseDTO>(mainManager);
                    allManagersDTO.Add(mainManagerDTO);
                }
            }

            return allManagersDTO;
        }


    }
}
