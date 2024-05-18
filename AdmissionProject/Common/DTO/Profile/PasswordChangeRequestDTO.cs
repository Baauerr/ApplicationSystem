namespace Common.DTO.Profile
{
    public class PasswordChangeRequestDTO
    {
        public required string CurrentPassword { get; set; }
        public required string Password { get; set; }
        public required string ConfirmPassword { get; set; }
    }

    public class PasswordChangeRequestRPCDTO
    {
        public PasswordChangeRequestDTO passwordInfo { get; set; }
        public Guid UserId { get; set; }
    }
}
