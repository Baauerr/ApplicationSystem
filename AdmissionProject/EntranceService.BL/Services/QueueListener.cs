using Common.Const;
using Common.DTO.Entrance;
using DocumentService.Common.DTO;
using EasyNetQ;
using EntranceService.Common.Interface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceService.BL.Services
{
    public static class QueueListener
    {

        public static void AddListeners(this IServiceCollection services)
        {
            IBus bus = RabbitHutch.CreateBus("host=localhost");
            var serviceProvider = services.BuildServiceProvider();

            var entrantService = serviceProvider.GetRequiredService<IEntrantService>();

            bus.PubSub.Subscribe<UpdateUserDataDTO>(QueueConst.UpdateUserDataQueue, data => entrantService.SyncUserDataInApplication(data));
        }
    }
}
