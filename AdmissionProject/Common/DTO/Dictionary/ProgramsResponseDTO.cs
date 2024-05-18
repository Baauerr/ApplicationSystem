using Common.Enum;

namespace Common.DTO.Dictionary
{

    public class ProgramResponseDTO
    {
        public List<ProgramsDTO> Programs { get; set; }
        public Pagination Pagination { get; set; }
    }

    public class ProgramsDTO
    {
        public string Name { get; set; }
        public string Code { get; set; }
        public Language Language { get; set; }
        public string EducationForm { get; set; }
        public FacultyDTO Faculty { get; set; }
        public EducationLevelDTO EducationLevel { get; set; }
        public Guid Id { get; set; }
        public DateTime CreateTime { get; set; }
    }
    public class Pagination
    {
        public int Size { get; set; }
        public int Count { get; set; }
        public int Current { get; set; }
    }
}
