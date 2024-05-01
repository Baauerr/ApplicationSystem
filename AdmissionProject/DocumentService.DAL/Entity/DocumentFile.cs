using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.DAL.Entity
{
    public class DocumentFile
    {
        public Guid Id { get; set; }
        public Guid OwnerId { get; set; }
        public string Path { get; set; }
    }
}
