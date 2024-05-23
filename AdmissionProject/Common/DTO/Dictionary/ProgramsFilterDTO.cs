using Common.Enum;

namespace Common.DTO.Dictionary
{
    public class ProgramsFilterDTO
    {
        public
        string? name{get; set;}
        public string? code{get; set;}

        public List<Language>? language{get; set;}

        public List<string>? educationForm{get; set;}

        public List<string>? educationLevel{get; set;}

        public List<string>? faculty{get; set;}
        public int page { get; set; } = 1;

        public int pageSize { get; set; } = 10;
    }
}
