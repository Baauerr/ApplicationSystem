using Common.Const;
using Common.DTO;
using Common.DTO.Dictionary;
using DictionaryService.Common.Interfaces;
using DocumentService.Common.DTO;
using EasyNetQ;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.BL.Services
{
    public static class QueueListener
    {
        public static void AddListeners(this IServiceCollection services)
        {
            var bus = RabbitHutch.CreateBus("host=localhost");
            var serviceProvider = services.BuildServiceProvider();

            var dictionaryService = serviceProvider.GetRequiredService<IDictionaryInfoService>();

            bus.Rpc.Respond<GetPrograms, ProgramResponseDTO>(async request =>
                await dictionaryService.GetPrograms(null, null, null, null, null, null, 1, 1000), x => x.WithQueueName(QueueConst.GetProgramsQueue)
            );

            bus.Rpc.Respond<GetEducationLevels, EducationLevelResponseDTO>(async request =>
                await dictionaryService.GetEducationLevel(null), x => x.WithQueueName(QueueConst.GetEducationLevelsQueue)
            );
        }
    }
}
