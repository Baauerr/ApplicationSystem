using DocumentService.Common.DTO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
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
        public Task DeletePassportFile(Guid userId);
        public Task UploadEducationDocumentFile(IFormFile file, Guid userId, EducationFileDTO educationLevelId);
        public Task DeleteEducationDocumentFile(Guid userId);
        public Task<FileStreamResult> GetPassportFile(Guid userId);
        public Task<FileStreamResult> GetEducationDocumentFile(Guid userId);

    }
}
