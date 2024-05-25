using Common.Const;
using Common.DTO.Dictionary;
using DictionaryService.Common.Interfaces;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;


namespace DictionaryService.BL.Services
{
    public static class QueueListener
    {
        public static void AddListeners(this IServiceCollection services)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            var serviceProvider = services.BuildServiceProvider();

            var dictionaryService = serviceProvider.GetRequiredService<IDictionaryInfoService>();
            var importService = serviceProvider.GetRequiredService<IImportService>();

            bus.Rpc.Respond<ProgramsFilterDTO, ProgramResponseDTO>(async request =>
                await dictionaryService.GetPrograms
                    (request.name,
                    request.code,
                    request.language,
                    request.educationForm,
                    request.educationLevel,
                    request.faculty,
                    request.page,
                    request.pageSize
                    ),
                x => x.WithQueueName(QueueConst.GetProgramsQueue)
            );

            bus.Rpc.Respond<Guid, EducationLevelResponseDTO>(async request =>
                await dictionaryService.GetEducationLevel(null), x => x.WithQueueName(QueueConst.GetEducationLevelsQueue)
            );

            bus.Rpc.Respond<Guid, FacultiesResponseDTO>(async request =>
                await dictionaryService.GetFaculties(null), x => x.WithQueueName(QueueConst.GetAllFacultiesQueue)
            );

            bus.Rpc.Respond<Guid, FacultiesResponseDTO>(async request =>
                await dictionaryService.GetFaculties(null), x => x.WithQueueName(QueueConst.GetAllFacultiesQueue)
            );

            bus.Rpc.Respond<Guid, AllImportHistoryDTO>(async request =>
                await importService.GetImportHistory(), x => x.WithQueueName(QueueConst.GetImportHistoryQueue)
            );

            bus.PubSub.Subscribe<MakeImportDTO>
                (QueueConst.UpdateUserDataQueue, data => importService.ImportDictionary(data.ImportTypes, data.UserId));
        }
    }
}
