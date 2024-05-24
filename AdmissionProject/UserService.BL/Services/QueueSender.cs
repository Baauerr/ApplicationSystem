using Common.Const;
using Common.DTO.Entrance;
using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UserService.Common.Interface;

namespace UserService.BL.Services
{
    public class QueueSender: IQueueSender
    {
        private IBus _bus;
        public QueueSender()
        {
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        public async Task SendMessage<T>(T message, string topik)
        {
            await _bus.PubSub.PublishAsync(message, topik);
        }

        public async Task SyncProfileInfo(UpdateUserDataDTO updateInfo)
        {
            await _bus.PubSub.PublishAsync(updateInfo, QueueConst.UpdateUserDataQueue);
        }
    }
          
}

