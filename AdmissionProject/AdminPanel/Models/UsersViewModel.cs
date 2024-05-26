using Common.DTO.User;

namespace AdminPanel.Models
{
    public class UsersViewModel
    {
        public UsersFilterDTO Filters { get; set; }
        public GetAllUsersDTO Users { get; set; }
    }

    public class UsersEditRoleModel
    {
        public UsersFilterDTO Filters { get; set; }
        public UserRoleActionDTO RoleAction { get; set; }
    }
}
