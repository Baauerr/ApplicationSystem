using System.ComponentModel.DataAnnotations;
using Common.Enum;

namespace EntranceService.DAL.Entity
{
    public class Application
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string OwnerName { get; set; }
        public string OwnerEmail { get; set; }
        public string Citizenship { get; set; }
        public DateTime LastChangeDate { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
        public Guid ManagerId { get; set; } = Guid.Empty;
        public string? ManagerEmail { get; set; }
        public string? ManagerFullName { get; set; }
    }
}
