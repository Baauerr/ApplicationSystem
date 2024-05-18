using Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Entrance
{
    public class ChangeApplicationStatusDTO
    {
        public ApplicationStatus ApplcationStatus {  get; set; }
        public Guid ApplicationId { get; set; }
    }
}
