using System.ComponentModel.DataAnnotations;
using UserService.Common.Enum;

namespace UserService.Common.DTO.Profile
{
    public class ChangeProfileRequestDTO
    {
        public required string FullName { get; set; }
        public DateTime? BirthDate { get; set; }
        public Gender? Gender { get; set; }
        public string? PhoneNumber { get; set; }
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Неправильный формат адреса электронной почты.")]
        public required string Email { get; set; }
        public string? Citizenship { get; set; }
    }
}
