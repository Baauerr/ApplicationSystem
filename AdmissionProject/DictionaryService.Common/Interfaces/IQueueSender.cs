using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.Common.Interfaces
{
    public interface IQueueSender
    {
        public Task SendMessage<T>(T message, string topik);
    }
}
