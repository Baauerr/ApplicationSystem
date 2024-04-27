using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Dictionary
{
    public class DocumentTypeResponseDTO
    {
        public List<DocumentTypeDTO> DocumentTypes { get; set; }
    }
    public class DocumentTypeDTO
    {
        public string Name { get; set; }
        public EducationLevelDTO EducationLevel { get; set; }
        public List<EducationLevelDTO> NextEducationLevels { get; set; }
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
