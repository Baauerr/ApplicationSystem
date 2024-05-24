using Common.DTO.Profile;
using Common.Enum;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Entrance
{
    public class EditManagerProfileDTO: ChangeProfileRequestDTO
    {
        public Guid UserId { get; set; }
    }
}
