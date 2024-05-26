using Common.Const;
using Common.DTO.Copy;
using Common.DTO.Entrance;
using EasyNetQ;
using EntranceService.Common.Interface;
using Microsoft.Extensions.DependencyInjection;

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

            bus.PubSub.Subscribe<List<Program>>
                (QueueConst.SyncApplicationWithProgramsQueue, data => entranceService.SyncApplicationsWithPrograms(data));

            bus.PubSub.Subscribe<RemoveApplicationManagerDTO>
                (QueueConst.RemoveManagerFromEntranceQueue, async data => await entranceService.RemoveManager(data.UserId));

            bus.PubSub.Subscribe<RefuseApplication>
                (QueueConst.RemoveApplicationManagerQueue, data => entranceService.RefuseApplication(data));

            bus.PubSub.Subscribe<TakeApplication>
                (QueueConst.SetManagerQueue, data => entranceService.SetManager(data));

            bus.PubSub.Subscribe<ChangeApplicationStatusDTO>
                (QueueConst.ChangeApplicationStatusQueue, data => entranceService.ChangeApplicationStatus(data.ApplcationStatus, data.ApplicationId));

            bus.PubSub.Subscribe<ManagerDTO>
                (QueueConst.CreateManagerProfileQueue, data => entranceService.CreateManager(data));

            bus.PubSub.Subscribe<ChangeProgramPriorityDTORPC>
                (QueueConst.ChangeProgramPriorityQueue, data => entranceService.ChangeProgramPriority(data.programInfo, data.UserId, data.ManagerId));

            bus.PubSub.Subscribe<DeleteProgramDTORPC>
                (QueueConst.RemoveProgramFromApplicationQueue, data => entranceService.DeleteProgram(data.deleteData, data.UserId, data.ManagerId));

            bus.PubSub.Subscribe<EduDocumentSyncApplicationDTO>
                (QueueConst.SyncApplicationWithEducationDocumentQueue, data => entranceService.SyncProgramsWithEducationDocument(data.UserId));

            bus.Rpc.Respond<Guid, ManagersListDTO>(info =>
                    entranceService.GetAllManagers(), x => x.WithQueueName(QueueConst.GetAllManagersQueue)
            );

            bus.Rpc.Respond<Guid, GetApplicationPrograms>(userId =>
                    entranceService.GetApplicationPrograms(userId), x => x.WithQueueName(QueueConst.GetApplicationProgramsQueue)
            );

            bus.Rpc.Respond<Guid, GetApplicationManagerId>(userId =>
                    entranceService.GetApplicationManagerId(userId), x => x.WithQueueName(QueueConst.GetApplicationManagerIdQueue)
            );

            bus.Rpc.Respond<ApplicationFiltersDTO, ApplicationsResponseDTO>(async filters =>
                await entranceService.GetApplications(
                    filters.entrantName,
                    filters.programsGuid,
                    filters.faculties, 
                    filters.status, 
                    filters.hasManager, 
                    filters.onlyMyManaging,
                    filters.managerId,
                    filters.sortingTypes,
                    filters.page,
                    filters.pageSize,
                    filters.myId
                ), 
                x => x.WithQueueName(QueueConst.GetApplicationsQueue)
            );
        }
    }
}
