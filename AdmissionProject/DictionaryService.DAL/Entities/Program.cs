using DictionaryService.Common.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.DAL.Entities
{
    public class Program
    {
        public string Id {  get; set; }
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Language Language { get; set; }
        public string EducationForm { get; set; }
        public string FacultyId { get; set; }
        public string EducationLevelId { get; set; }
    }
}
