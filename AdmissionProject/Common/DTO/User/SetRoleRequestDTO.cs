

using Common.Enum;

namespace Common.DTO.User
{
    public class SetRoleRequestDTO
    {
        public required string RecipientId { get; set; }
        public required Roles Role { get; set; }

    }
}
