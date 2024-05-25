using Common.DTO.Profile;
using Common.DTO.Document;
using Common.DTO.Dictionary;

namespace AdminPanel.Models
{
    public class EducationDocumentViewModel
    {
        public GetEducationDocumentFormDTO EducationDocumentForm { get; set; }
        public EducationLevelResponseDTO EducationLevel { get; set; }
    }
}
