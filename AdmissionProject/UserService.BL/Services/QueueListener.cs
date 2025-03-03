﻿using Common.Const;
using Common.DTO.Auth;
using Common.DTO.Profile;
using Common.DTO.User;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using UserService.Common.Interface;

namespace UserService.BL.Services
{
    public static class QueueListener
    {
        public static void AddListeners(this IServiceCollection services)
        {

            IBus bus = RabbitHutch.CreateBus("host=localhost");
            var serviceProvider = services.BuildServiceProvider();

            var accountService = serviceProvider.GetRequiredService<IAccountService>();
            var authService = serviceProvider.GetRequiredService<IAuthService>();

            bus.Rpc.Respond<Guid, ProfileResponseDTO>(async request =>
                  await accountService.GetProfile(request), x => x.WithQueueName(QueueConst.GetProfileInfoQueue)
            );

            bus.Rpc.Respond<LoginRequestDTO, AuthResponseDTO>(async request =>
                  await authService.Login(request), x => x.WithQueueName(QueueConst.LoginQueue)
            );

            bus.Rpc.Respond<UsersFilterDTO, GetAllUsersDTO>(async request =>
                  await accountService.GetAllUsers(request), x => x.WithQueueName(QueueConst.GetAllUsersQueue)
            );

            bus.PubSub.Subscribe<ChangeProfileRequestRPCDTO>
                (QueueConst.ChangeProfileQueue, data => accountService.ChangeProfileInfo(data.ProfileData, data.UserId));

            bus.PubSub.Subscribe<LogoutDTO>
                (QueueConst.LogoutQueue, data => authService.Logout(data.Token));

            bus.PubSub.Subscribe<PasswordChangeRequestRPCDTO>
                (QueueConst.ChangePasswordQueue, data => authService.ChangePassword(data.passwordInfo, data.UserId));

            bus.PubSub.Subscribe<UserRoleActionDTO>(QueueConst.SetRoleQueue, data => accountService.GiveRole(data));
            bus.PubSub.Subscribe<DeleteUserRoleDTO>(QueueConst.RemoveUserRoleQueue, data => accountService.RemoveRole(data));

        }
    }
}
