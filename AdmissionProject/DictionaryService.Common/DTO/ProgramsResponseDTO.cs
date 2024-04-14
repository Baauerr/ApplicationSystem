using DictionaryService.Common.Enums;
using DictionaryService.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DictionaryService.Common.DTO
{

    public class ProgramResponseDTO
    {
        public List<ProgramsDTO> programs;
        public Pagination pagination;
    }

    public class ProgramsDTO
    {
        public string Name;
        public string Code;
        public Language Language;
        public string EducationForm;
        public Faculty Faculty;
        public EducationLevel EducationLevel;
        public string Id;
        public DateTime CreateTime;
    }
    public class Pagination
    {
        public int size;
        public int count;
        public int current;
    }
}
