using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Entrance
{
    public class UpdateUserDataDTO
    {
        public string NewUserName { get; set; }
        public string NewEmail { get; set; }
        public Guid UserId { get; set; }
    }
}
