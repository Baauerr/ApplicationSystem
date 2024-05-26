using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Entrance
{
    public class ChangeProgramPriorityDTO
    {
        public int Priority { get; set; }
        public Guid ProgramId { get; set; }
    }

    public class ChangeProgramPriorityDTORPC
    {
        public ChangeProgramPriorityDTO programInfo { get; set; }
        public Guid UserId { get; set; }
        public Guid ManagerId { get; set; }
    }
}
