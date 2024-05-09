using Common.DTO.Dictionary;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceService.Common.Interface
{
    public interface IQueueSender
    {
        public Task SendMessage<T>(T message);
    }
}
