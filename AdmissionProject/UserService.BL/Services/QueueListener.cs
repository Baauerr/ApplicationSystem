using Common.Const;
using Common.DTO;
using Common.DTO.Entrance;
using Common.DTO.Profile;
using Common.DTO.User;
using DocumentService.Common.DTO;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Common.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserService.BL.Services
{
    public static class QueueListener
    {
        public static void AddListeners(this IServiceCollection services)
        {

            IBus bus = RabbitHutch.CreateBus("host=localhost");
            var serviceProvider = services.BuildServiceProvider();

            var accountService = serviceProvider.GetRequiredService<IAccountService>();

            bus.Rpc.Respond<Guid, ProfileResponseDTO>(async request =>
                  await accountService.GetProfile(request), x => x.WithQueueName(QueueConst.GetProfileInfoQueue)
            );

            bus.PubSub.Subscribe<SetRoleRequestDTO>(QueueConst.SetRoleQueue, data => accountService.GiveRole(data));

        }
    }
}
