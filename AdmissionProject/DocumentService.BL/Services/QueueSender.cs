using Common.Const;
using Common.DTO;
using Common.DTO.Dictionary;
using DocumentService.Common.DTO;
using EasyNetQ;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.BL.Services
{
    public class QueueSender
    {
        private IBus _bus;
        public QueueSender()
        {
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        public EducationLevelResponseDTO GetEducationLevels()
        {
            var educationLevels = _bus.Rpc.Request<Guid, EducationLevelResponseDTO>
                (Guid.Empty, x => x.WithQueueName(QueueConst.GetEducationLevelsQueue));

            return educationLevels;
        }
    }
}
