using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Dictionary
{
    public class FacultiesResponseDTO
    {
        public List<FacultyDTO> Faculties { get; set; }
    }

    public class FacultyDTO
    {
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
    }

}
