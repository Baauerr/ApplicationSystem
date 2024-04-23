using EntranceService.DAL.Enum;
using System.ComponentModel.DataAnnotations;


namespace EntranceService.DAL.Entity
{
    public class Application
    {
        [Key]
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public required string Citizenship { get; set; }
        public required string Gender { get; set; }
        public required string PhoneNumber { get; set; }
        public Guid EducationDocumentId { get; set; }
        public DateTime LastChangeDate { get; set; }
        public ApplicationStatus ApplicationStatus { get; set; }
    }
}
