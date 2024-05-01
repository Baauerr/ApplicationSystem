using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Common.Interface
{
    public interface IFileService
    {
        public Task UploadPassportFile(IFormFile file, Guid userId);
        public Task DeletePassportFile();
        public Task UploadEducationDocumentFile(IFormFile file, Guid userId, string educationLevelId);
        public Task DeleteEducationDocumentFile();
    }
}
