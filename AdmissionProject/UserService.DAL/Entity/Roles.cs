using System.ComponentModel.DataAnnotations;

namespace UserService.DAL.Entity
{
    public enum Roles
    {
        [Display(Name = RoleNames.Admin)]
        Admin,
        [Display(Name = RoleNames.Manager)]
        Manager,
        [Display(Name = RoleNames.MainManager)]
        MainManager,
        [Display(Name = RoleNames.User)]
        User,
        [Display(Name = RoleNames.Entrant)]
        Entrant
    }

    public class RoleNames
    {
        public const string Admin = "Admin";
        public const string Manager = "Manager";
        public const string MainManager = "MainManager";
        public const string User = "User";
        public const string Entrant = "Entrant";
    }
}
