using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common.DTO
{
    public class GetProfile
    {
        public Guid UserId { get; set; }
    }

    public class GetPrograms
    {
        public Guid UserId { get; set; }
    }

    public class GetPassportInfo
    {
        public Guid UserId { get; set; }
    }

    public class GetEducationLevels
    {
        public Guid UserId { get; set; }
    }

    public class GetEducationDocumentForm
    {
        public Guid UserId { get; set; }
    }

}
