using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.DAL.Entities
{
    public class Program
    {
        public Guid Id {  get; set; }
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public string EducationForm { get; set; }
        public string FacultyId { get; set; }
        public string EducationLevelId { get; set; }
    }
}
