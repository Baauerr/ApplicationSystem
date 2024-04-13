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
        public List<DocumentTypeDTO> DocumentTypes;
    }
    public class DocumentTypeDTO
    {
        public string Name;
        public EducationLevel EducationLevel;
        public List<EducationLevel> NextEducationLevels;
        public Guid Id;
        public DateTime CreateTime;
    }
}
