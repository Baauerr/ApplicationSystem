using Common.DTO.Document;
using Common.DTO.Entrance;
using Common.DTO.Profile;


namespace AdminPanel.Models
{
    public class EntrantViewModel
    {
        public ProfileResponseDTO Profile { get; set; }
        public PassportFormDTO PassportForm { get; set; }
        public EducationDocumentViewModel EducationDocumentForm { get; set; }
        public GetApplicationPrograms ApplicationPrograms { get; set; }
        public Guid EntrantId { get; set; }
    }
}
