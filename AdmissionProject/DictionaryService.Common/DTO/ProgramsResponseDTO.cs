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
        public List<ProgramsDTO> programs { get; set; }
        public Pagination pagination { get; set; }
    }

    public class ProgramsDTO
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Language Language { get; set; }
        public string EducationForm { get; set; }
        public Faculty Faculty { get; set; }
        public EducationLevelDTO EducationLevel { get; set; }
        public string Id { get; set; }
        public DateTime CreateTime { get; set; }
    }
    public class Pagination
    {
        public int size { get; set; }
        public int count { get; set; }
        public int current { get; set; }
    }
}
