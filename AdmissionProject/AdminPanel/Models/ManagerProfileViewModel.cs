using Common.DTO.Profile;

namespace AdminPanel.Models
{
    public class ManagerProfileViewModel
    {
        public ProfileResponseDTO Profile { get; set; }
        public Guid ManagerId { get; set; }
    }
}
