using Common.DTO.Dictionary;
using Common.DTO.Entrance;

namespace AdminPanel.Models
{
    public class ApplicationsViewModel
    {
        public ApplicationFiltersDTO Filters { get; set; }
        public ApplicationsResponseDTO ApplicationsResponse { get; set; }
        public FacultiesResponseDTO Faculties { get; set; }
        public ProgramResponseDTO Programs { get; set; }
        public Guid myId { get; set; }
    }
}
