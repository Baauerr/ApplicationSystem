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
            var entranceService = serviceProvider.GetRequiredService<IEntranceService>();

            bus.PubSub.Subscribe<UpdateUserDataDTO>
                (QueueConst.UpdateUserDataQueue, data => entrantService.SyncUserDataInApplication(data));

            bus.PubSub.Subscribe<RefuseApplication>
                (QueueConst.RemoveApplicationManagerQueue, data => entranceService.RefuseApplication(data));

            bus.PubSub.Subscribe<TakeApplication>
                (QueueConst.SetManagerQueue, data => entranceService.SetManager(data));

            bus.PubSub.Subscribe<ChangeApplicationStatusDTO>
                (QueueConst.ChangeApplicationStatusQueue, data => entranceService.ChangeApplicationStatus(data.ApplcationStatus, data.ApplicationId));

            bus.Rpc.Respond<ApplicationFiltersDTO, ApplicationsResponseDTO>(async filters =>
                await entranceService.GetApplications(
                    filters.entrantName,
                    filters.programsGuid,
                    filters.faculties, 
                    filters.status, 
                    filters.hasManager, 
                    filters.managerName,
                    filters.sortingTypes,
                    filters.page,
                    filters.pageSize
                ), 
                x => x.WithQueueName(QueueConst.GetApplicationsQueue)
            );
        }
    }
}
