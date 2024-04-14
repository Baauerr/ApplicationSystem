using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.DAL.Entities
{
    public class DocumentType
    {
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
        public string EducationLevelId { get; set; }
        public ICollection<NextEducationLevel> NextEducationLevels { get; set; }
    }
}
