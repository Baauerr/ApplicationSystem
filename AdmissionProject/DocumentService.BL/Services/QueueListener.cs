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

            bus.Rpc.Respond<GetPassportInfo, GetPassportFormDTO>(async request =>
                await documentFormService.GetPassportInfo(request.UserId)
            );

            bus.Rpc.Respond<GetEducationDocumentForm, List<GetEducationDocumentFormDTO>>(async request =>
                await documentFormService.GetEducationDocumentsInfo(request.UserId)
            );
            
        }
    }
}
