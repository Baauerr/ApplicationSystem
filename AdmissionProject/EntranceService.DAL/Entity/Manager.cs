using Common.Enum;

namespace EntranceService.DAL.Entity
{
    public class Manager
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } 
        public string Email { get; set; }
        public Roles Role { get; set; }
    }
}
