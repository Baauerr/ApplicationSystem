using Common.Enum;

namespace Common.DTO.Entrance
{
    public class CreateApplicationDTO
    {
        public required string Citizenship {  get; set; }
        public List<ProgramDTO> Programs { get; set; }
    }
}
