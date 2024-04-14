using System.ComponentModel.DataAnnotations;

namespace DictionaryService.DAL.Entities
{
    public class EducationLevel
    {
        [Key]
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<NextEducationLevel> DocumentTypes { get; set; }
    }
}
