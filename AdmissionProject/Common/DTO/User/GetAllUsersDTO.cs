using Common.DTO.Dictionary;
using Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.User
{
    public class UsersFilterDTO
    {
        public int NumberOfUsers { get; set; } = 10;
        public string? FullName { get; set; }
        public int Page { get; set; } = 1;
    }

    public class UserDTO
    {
        public Guid Id { get; set; }
        public string Email { get; set; }
        public IList<string> Roles {  get; set; }
        public string FullName { get; set; }
    }

    public class GetAllUsersDTO
    {
        public List<UserDTO> Users { get; set; } = new List<UserDTO>();

    }
}
