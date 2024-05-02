using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Common.DTO
{
    public class PassportFormDTO
    {
        public int Series { get; set; }
        public int Number { get; set; }
        public string BirthPlace { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuePlace { get; set; }
    }


    public class GetPassportFormDTO: PassportFormDTO
    {
        public Guid UserId { get; set; }
    }

}
