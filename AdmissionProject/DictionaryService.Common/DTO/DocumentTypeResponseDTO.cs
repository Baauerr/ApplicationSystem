using DictionaryService.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.Common.DTO
{
    public class DocumentTypeResponseDTO
    {
        public List<DocumentTypeDTO> DocumentTypes { get; set; }
    }
    public class DocumentTypeDTO
    {
        public string Name { get; set; }
        public EducationLevel EducationLevel { get; set; }
        public List<EducationLevel> NextEducationLevels { get; set; }
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
    }
}
