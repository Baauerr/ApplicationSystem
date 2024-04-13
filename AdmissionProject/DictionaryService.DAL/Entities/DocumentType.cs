using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.DAL.Entities
{
    public class DocumentType
    {
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
        public Guid EducationLevelId { get; set; }
        public List<Guid> NextEducationLevelId { get; set; }
    }
}
