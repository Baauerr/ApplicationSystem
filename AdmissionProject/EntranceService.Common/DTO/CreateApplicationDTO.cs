using Common.Enum;

namespace EntranceService.Common.DTO
{
    public class CreateApplicationDTO
    {
        public string Name { get; set; }
        public string MiddleName { get; set; }
        public string LastName { get; set; }
        public string Citizenship { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
        public List<ProgramDTO> ProgramList { get; set; }
    }
}
