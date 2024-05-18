using Common.Enum;
using System.ComponentModel.DataAnnotations;

namespace Common.DTO.Profile
{
    public class ChangeProfileRequestDTO
    {
        public required string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender Gender { get; set; }
        public string? PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Неправильный формат адреса электронной почты.")]
        public required string Email { get; set; }
        public string? Citizenship { get; set; }
    }

    public class ChangeProfileRequestRPCDTO
    {
        public ChangeProfileRequestDTO ProfileData { get; set; }
        public Guid UserId { get; set; }
    }
}
