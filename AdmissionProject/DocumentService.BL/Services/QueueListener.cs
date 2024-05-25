using Common.Const;
using Common.DTO.Document;
using Common.DTO.Entrance;
using DocumentService.Common.Interface;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;


namespace DocumentService.BL.Services
{

    
    public static class QueueListener
    {
        public static void AddListeners(this IServiceCollection services)
        {

            IBus bus = RabbitHutch.CreateBus("host=localhost");
            var serviceProvider = services.BuildServiceProvider();

            var documentFormService = serviceProvider.GetRequiredService<IDocumentFormService>();

            bus.Rpc.Respond<Guid, GetPassportFormDTO>(async userId =>
                await documentFormService.GetPassportInfo(userId), x => x.WithQueueName(QueueConst.GetPassportFormQueue)
            );

            bus.Rpc.Respond<Guid, GetEducationDocumentFormDTO>(async userId =>
                await documentFormService.GetEducationDocumentInfo(userId), x => x.WithQueueName(QueueConst.GetEducationDocumentsFormsQueue)
            );

            bus.PubSub.Subscribe<EditPassportFormDTORPC>
                (QueueConst.UpdateEntrantPassportQueue, data => documentFormService.EditPassportInfo(data.PassportInfo, data.UserId));

            bus.PubSub.Subscribe<EditEducationDocumentFormRPC>
                (QueueConst.UpdateEducationDocumentFormQueue, data => documentFormService.EditEducationDocumentInfo(data.EducationDocumentInfo, data.UserId));

        }
    }
}
