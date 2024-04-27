using Common.Enum;

namespace DictionaryService.DAL.Entities
{
    public class Program
    {
        public Guid Id {  get; set; }
        public DateTime CreateTime { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public Language Language { get; set; }
        public string EducationForm { get; set; }
        public string FacultyId { get; set; }
        public string EducationLevelId { get; set; }
    }
}
