using EntranceService.DAL.Enum;
using System.ComponentModel.DataAnnotations;


namespace EntranceService.DAL.Entity
{
    public class Application
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Citizenship { get; set; }
        public DateTime LastChangeDate { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
        public Guid ManagerId { get; set; }
    }
}
