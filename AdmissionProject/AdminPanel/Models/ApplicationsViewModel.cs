using Common.DTO.Entrance;

namespace AdminPanel.Models
{
    public class ApplicationsViewModel
    {
        public ApplicationFiltersDTO Filters { get; set; }
        public ApplicationsResponseDTO ApplicationsResponse { get; set; }
        public Guid myId { get; set; }
    }
}
