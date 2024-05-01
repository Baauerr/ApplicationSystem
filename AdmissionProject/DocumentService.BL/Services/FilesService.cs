using DocumentService.Common.Interface;
using DocumentService.DAL;
using DocumentService.DAL.Entity;
using Exceptions.ExceptionTypes;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.BL.Services
{
    public class FilesService: IFileService
    {
        private readonly DocumentDbContext _db;


        public FilesService(DocumentDbContext documentDbContext, IDocumentFormService documentFormService) {
            _db = documentDbContext; 
        }

        public async Task UploadPassportFile(IFormFile file, Guid userId)
        {
            var passportDirectoryPath = "C:\\Users\\Артем\\Desktop\\ASPNET\\AdmissionProject\\documentFiles\\passport\\";
            //var passportForm = await _db.PassportsData.FirstOrDefaultAsync(pf => pf.OwnerId == userId);
          // if (passportForm == null) {
             //   throw new BadRequestException("Нельзя загрузить файл, если нет формы");
           // }
            var passportFilePath = passportDirectoryPath + userId.ToString() + ".pdf";

            var passportFile = new DocumentFile
            {
                Path = passportFilePath,
                OwnerId = userId,
            };

            using (var stream = new FileStream(passportFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

          //  await _db.PassportsFiles.AddAsync(passportFile);
          //  await _db.SaveChangesAsync();
        }

        public async Task DeletePassportFile()
        {

        }

        public async Task UploadEducationDocumentFile(IFormFile file, Guid userId, string educationLevelId)
        {
            var educationDocumentDirectoryPath = "C:\\Users\\Артем\\Desktop\\ASPNET\\AdmissionProject\\documentFiles\\educationDocuments";

          //  var educationDocumentForm = await _db.EducationDocumentsData.FirstOrDefaultAsync(ed => ed.OwnerId == userId && ed.EducationLevelId == educationLevelId);
            
           // if (educationDocumentForm == null)
          //  {
         //       throw new BadRequestException("Нельзя загрузить файл, если нет формы");
         //   }
            var educationDocumentFilePath = educationDocumentDirectoryPath + userId.ToString() + "_" + educationLevelId;

            var educationDocumentFile = new DocumentFile
            {
                Path = educationDocumentFilePath,
                OwnerId = userId,
            };

            using (var stream = new FileStream(educationDocumentFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            await _db.EducationDocumentsFiles.AddAsync(educationDocumentFile);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteEducationDocumentFile()
        {

        }
    }
}
