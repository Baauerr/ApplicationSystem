using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Entrance
{
    public class AddProgramsToApplicationDTO
    {
        public List<ProgramDTO> programsDTO {  get; set; }
        public Guid applicationId { get; set; }

    }
}
