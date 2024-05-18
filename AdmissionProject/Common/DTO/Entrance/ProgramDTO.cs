using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Entrance
{
    public class ProgramDTO
    {
        public Guid ProgramId { get; set; }
        public string FacultyId { get; set; }
        public int ProgramPriority { get; set; }
    }
    public class GetProgramDTO : ProgramDTO
    {
        public string ProgramName { get; set; }
        public string FacultyName { get; set; }
    }
        public class AddProgramDTO
        {
            public required List<ProgramDTO> Programs { get; set; }
            public Guid ApplicationId { get; set; }
        }

        public class ProgramChangePriorityDTO
        {
            public Guid ApplicationId { get; set; }
            public Guid ProgramGuid { get; set; }
            public int NewPriority { get; set; }
            public int OldPriority { get; set; }
        }

        public class DeleteProgramDTO
        {
            public Guid ProgramId { get; set; }
            public Guid ApplicationId { get; set; }
        }
    
}
