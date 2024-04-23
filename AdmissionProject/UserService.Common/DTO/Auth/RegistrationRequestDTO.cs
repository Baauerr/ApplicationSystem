using Common.Enum;
using System.ComponentModel.DataAnnotations;
using UserService.Common.Enum;

namespace UserService.Common.DTO.Auth
{
    public class RegistrationRequestDTO
    {
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Неправильный формат адреса электронной почты.")]
        public string Email { get; set; }
        public string Password { get; set; }
        public DateTime BirthDate { get; set; }
        public string FullName { get; set; }
        public Gender Gender { get; set; }
        public string PhoneNumber { get; set; }
    }
}
