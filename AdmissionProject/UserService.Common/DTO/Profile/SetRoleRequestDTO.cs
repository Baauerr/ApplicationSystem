using UserService.Common.Enum;

namespace UserService.Common.DTO.Profile
{
    public class SetRoleRequestDTO
    {
        public required string RecipientId { get; set; }
        public required Roles Role { get; set; }

    }
}
