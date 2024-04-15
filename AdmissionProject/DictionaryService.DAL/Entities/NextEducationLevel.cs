using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.DAL.Entities
{
    public class NextEducationLevel
    {
        public string DocumentTypeId { get; set; }
        public string EducationLevelId { get; set; }
        public string EducationLevelName { get; set; }
    }
}
