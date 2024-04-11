namespace UserService.Common.DTO.Profile
{
    public class PasswordChangeRequestDTO
    {
        public required string Password { get; set; }
        public required string ConfrimPassword { get; set; }
    }
}
