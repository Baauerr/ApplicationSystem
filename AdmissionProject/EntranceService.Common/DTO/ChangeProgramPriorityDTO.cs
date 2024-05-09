using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceService.Common.DTO
{
    public class ChangeProgramPriorityDTO
    {
        public Guid ApplicationId { get; set; }
        public int Priority { get; set; }
        public Guid ProgramId { get; set; }
    }
}
