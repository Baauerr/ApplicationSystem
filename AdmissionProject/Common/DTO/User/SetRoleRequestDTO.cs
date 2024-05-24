using Common.Enum;

namespace Common.DTO.User
{
    public class UserRoleActionDTO
    {
        public required Guid UserId { get; set; }
        public required Roles Role { get; set; }
    }
    public class DeleteUserRoleDTO: UserRoleActionDTO
    {

    }

}
