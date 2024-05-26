using AutoMapper;
using Common.Const;
using Common.DTO.Entrance;
using Common.DTO.Profile;
using Common.DTO.User;
using Common.Enum;
using EasyNetQ;
using Exceptions.ExceptionTypes;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.SignalR.Protocol;
using Microsoft.EntityFrameworkCore;
using UserService.Common.DTO.Profile;
using UserService.Common.Interface;
using UserService.DAL;
using UserService.DAL.Entity;

namespace UserService.BL.Services
{
    public class AccountService : IAccountService
    {
        private readonly UserManager<User> _userManager;
        private readonly UserDbContext _db;
        private readonly SignInManager<User> _signInManager;
        private readonly ITokenService _tokenService;
        private readonly IQueueSender _queueSender;
        private readonly IMapper _mapper;

        public AccountService(
            UserManager<User> userManager, ITokenService tokenService, IMapper mapper,
            IQueueSender queueSender, SignInManager<User> signInManager, UserDbContext db
            ) {
            _userManager = userManager;
            _tokenService = tokenService;
            _mapper = mapper;
            _queueSender = queueSender;
            _signInManager = signInManager;
            _db = db;
        }
        public async Task ChangeProfileInfo(ChangeProfileRequestDTO newProfileInfo, Guid userId)
        {
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
            user.Gender = newProfileInfo.Gender;
            user.Citizenship = newProfileInfo.Citizenship;

            await SyncProfileInfo(userId, newProfileInfo.FullName, newProfileInfo.Email);

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

        private async Task SyncProfileInfo(Guid userId, string newName, string newEmail)
        {
            var updateInfo = new UpdateUserDataDTO
            {
                UserId = userId,
                NewUserName = newName,
                NewEmail = newEmail,
            };

            await _queueSender.SendMessage(updateInfo, QueueConst.UpdateUserDataQueue);
        }

        public async Task<ProfileResponseDTO> GetProfile(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            var userProfile = _mapper.Map<ProfileResponseDTO>(user);
            return userProfile;
        }
        public async Task<UserRoleResponseDTO> GetMyRoles(Guid userId)
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());

            if (user == null) throw new NotFoundException("Такого пользователя не существует");

            var roles = await _userManager.GetRolesAsync(user);

            var rolesDTO = new UserRoleResponseDTO
            {
                roles = roles
            };

            return rolesDTO;
        }

        public async Task GiveRole(UserRoleActionDTO roleRequesData)
        {
            var role = roleRequesData.Role;
            var user = await _db.Users.FirstOrDefaultAsync(us => us.Id == roleRequesData.UserId);
            if (user == null) throw new NotFoundException("Такого пользователя не существует");
            await _userManager.AddToRoleAsync(user, role.ToString());

            if (roleRequesData.Role == Roles.MAINMANAGER)
            {
                await _userManager.AddToRoleAsync(user, Roles.MANAGER.ToString());
            }

            if (roleRequesData.Role == Roles.MAINMANAGER || roleRequesData.Role == Roles.MANAGER)
            {
                await CreateManagerProfile(user, roleRequesData.Role); 
            }

        }

        private async Task CreateManagerProfile(User userInfo, Roles role)
        {
            var managerProfile = new ManagerDTO
            {
                Email = userInfo.Email,
                FullName = userInfo.FullName,
                Id = userInfo.Id,
                Role = role
            };

            await _queueSender.SendMessage(managerProfile, QueueConst.CreateManagerProfileQueue);
        }

        public async Task RemoveRole(DeleteUserRoleDTO userInfo)
        {
            var user = await _db.Users.FirstOrDefaultAsync(us => us.Id == userInfo.UserId);

            if (user != null)
            {

                IEnumerable<string> role = new List<string> { userInfo.Role.ToString() };
                var result = await _userManager.RemoveFromRolesAsync(user, role);

                if (result.Succeeded)
                {
                    if (userInfo.Role == Roles.MANAGER)
                    {

                        var removeManagerDTO = new RemoveApplicationManagerDTO
                        {
                            UserId = userInfo.UserId
                        };

                        await _queueSender.SendMessage(removeManagerDTO, QueueConst.RemoveManagerFromEntranceQueue);
                        var isMainManager = await _userManager.IsInRoleAsync(user, Roles.MAINMANAGER.ToString());
                        if (isMainManager)
                        {
                            IEnumerable<string> mainManagerRole = new List<string> { Roles.MAINMANAGER.ToString() };
                            await _userManager.RemoveFromRolesAsync(user, mainManagerRole);
                        }
                    }
                    await _db.SaveChangesAsync();
                }
                else
                {
                    var errors = string.Join("; ", result.Errors.Select(e => e.Description));
                    throw new BadRequestException($"О: {errors}");
                }
            }   
        }

        public async Task<GetAllUsersDTO> GetAllUsers(UsersFilterDTO filters)
        {
            var usersDTO = new GetAllUsersDTO();

            var query = _db.Users.AsQueryable();

            if (filters.FullName != null)
            {
                query = query.Where(u => u.FullName.Contains(filters.FullName));
            }

            int skipAmount = (filters.Page - 1) * filters.NumberOfUsers;

            var filteredUsers = new List<User>();

            if (query.Count() < skipAmount)
            {
                filteredUsers = query
                    .Skip(skipAmount)
                    .Take(filters.NumberOfUsers)
                    .ToList();
            }
            else
            {
                filteredUsers = query.ToList();
            }

            foreach (var user in filteredUsers)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var userDTO = _mapper.Map<UserDTO>(user);
                userDTO.Roles = userRoles;
                if (userRoles.Contains("Administrator") || userRoles.Contains("ADMINISTRATOR"))
                {
                    continue;
                }
                usersDTO.Users.Add(userDTO);
            }

            usersDTO.Users.OrderByDescending(u => u.FullName);

            return usersDTO;
        }
        
    }
}
