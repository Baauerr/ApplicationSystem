using Common.Enum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO.Dictionary
{
    public class MakeImportDTO
    {
        public ImportTypes ImportTypes { get; set; }
        public Guid UserId { get; set; }
    }
}
