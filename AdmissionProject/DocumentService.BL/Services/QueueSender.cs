﻿using Common.Const;
using Common.DTO.Dictionary;
using EasyNetQ;
using Exceptions.ExceptionTypes;

namespace DocumentService.BL.Services
{
    public class QueueSender
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

        public EducationLevelResponseDTO GetEducationLevels()
        {
            try
            {
                var educationLevels = _bus.Rpc.Request<Guid, EducationLevelResponseDTO>
                                (Guid.Empty,
                                x => x.WithQueueName(QueueConst.GetEducationLevelsQueue));

                return educationLevels;
            }
            catch (Exception ex)
            {                
                throw new BadRequestException(ex.ToString());
            }
        }
    }
}
