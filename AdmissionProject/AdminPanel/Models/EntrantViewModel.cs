using Common.DTO.Profile;
using DocumentService.Common.DTO;

namespace AdminPanel.Models
{
    public class EntrantViewModel
    {
        public ProfileResponseDTO Profile { get; set; }
        public PassportFormDTO PassportForm { get; set; }
        public GetEducationDocumentFormDTO EducationDocumentForm { get; set; }
    }
}
