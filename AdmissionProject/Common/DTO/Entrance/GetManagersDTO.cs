using Common.Enum;

namespace Common.DTO.Entrance
{
    public class ManagersListDTO
    {
        public List<ManagerDTO> ManagerDTO { get; set; } = new List<ManagerDTO>();
    }

    public class ManagerDTO
    {
        public string FullName { get; set; }
        public Guid Id { get; set; }
        public string Email { get; set; }
        public Roles Role { get; set; }
    }
}
