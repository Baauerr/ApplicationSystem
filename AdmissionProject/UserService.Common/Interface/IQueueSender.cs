using Common.DTO.Entrance;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UserService.Common.Interface
{
    public interface IQueueSender
    {
        public Task SendMessage<T>(T message, string topik);
        public Task SyncProfileInfo(UpdateUserDataDTO updateInfo);
    }
}
