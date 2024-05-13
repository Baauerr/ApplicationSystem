using Common.Const;
using Common.DTO;
using DocumentService.Common.DTO;
using DocumentService.Common.Interface;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            
        }
    }
}
