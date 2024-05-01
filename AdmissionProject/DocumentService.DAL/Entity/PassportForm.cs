using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.DAL.Entity
{
    public class PassportForm
    {
        [Key]
        public Guid OwnerId { get; set; }
        public int Series { get; set; }
        public int Number { get; set; }
        public string BirthPlace { get; set; }
        public DateTime IssueDate { get; set; }
        public string IssuePlace { get; set; }
    }
}
