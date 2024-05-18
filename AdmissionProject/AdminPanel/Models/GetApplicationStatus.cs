using Common.DTO.Entrance;
using Common.Enum;

namespace AdminPanel.Models
{
    public class GetApplicationStatus
    {
        public Guid applicationId { get; set; }
        public ApplicationStatus status { get; set; }
    }
    
}
