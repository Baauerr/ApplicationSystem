using Common.Enum;

namespace EntranceService.Common.DTO
{
    public class CreateApplicationDTO
    {
        public required string Citizenship {  get; set; }
        public List<ProgramDTO> Programs { get; set; }
    }
}
