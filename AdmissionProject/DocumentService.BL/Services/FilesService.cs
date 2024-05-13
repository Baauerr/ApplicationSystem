using DocumentService.Common.DTO;
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


        public FilesService(DocumentDbContext documentDbContext) {
            _db = documentDbContext; 
        }

        public async Task UploadPassportFile(IFormFile file, Guid userId)
        {
            var passportDirectoryPath = Environment.CurrentDirectory + "\\documentFiles\\passport\\";
            
            var passportForm = await _db.PassportsData.FirstOrDefaultAsync(pf => pf.OwnerId == userId);
            
            if (passportForm == null) {
                throw new BadRequestException("Нельзя загрузить файл, если нет формы");
            }

            
            var passportFilePath = passportDirectoryPath + userId.ToString() + ".pdf";

            var fileId = Guid.NewGuid();

            passportForm.fileId = fileId;

            var passportFile = new PassportFile
            {
                Id = fileId,
                Path = passportFilePath,
                OwnerId = userId,
            };

            using (var stream = new FileStream(passportFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            await _db.PassportsFiles.AddAsync(passportFile);
            _db.PassportsData.Update(passportForm);
            await _db.SaveChangesAsync();
        }

        public async Task DeletePassportFile(Guid userId)
        {
            var passportFile = await _db.PassportsFiles
                .FirstOrDefaultAsync(pf => pf.OwnerId == userId);

            var passportForm = await _db.PassportsData
                .FirstOrDefaultAsync(pd => pd.OwnerId == userId);

            if (passportFile == null)
            {
                throw new NotFoundException("У пользователя нет сканов паспорта");
            }


            passportForm.fileId = Guid.Empty;

            _db.PassportsFiles.Remove(passportFile);
            _db.PassportsData.Update(passportForm);
            await _db.SaveChangesAsync();

            File.Delete(passportFile.Path);
        }

        public async Task UploadEducationDocumentFile(IFormFile file, Guid userId, EducationFileDTO educationLevelId)
        {
            var educationDocumentDirectoryPath = Environment.CurrentDirectory + "\\documentFiles\\educationDocuments";

            var educationDocumentForm = await _db.EducationDocumentsData.FirstOrDefaultAsync(ed => ed.OwnerId == userId && ed.EducationLevelId == educationLevelId.EducationLevelId);
            
            if (educationDocumentForm == null)
            {
                throw new BadRequestException("Нельзя загрузить файл, если нет формы");
            }
            var educationDocumentFilePath = educationDocumentDirectoryPath + userId.ToString() + "_" + educationLevelId;

            var fileId = Guid.NewGuid();

            educationDocumentForm.fileId = fileId;

            var educationDocumentFile = new EducationDocumentFile
            {
                Id = fileId,
                Path = educationDocumentFilePath,
                OwnerId = userId,
                EducationLevelId = educationLevelId.EducationLevelId
            };

            using (var stream = new FileStream(educationDocumentFilePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            await _db.EducationDocumentsFiles.AddAsync(educationDocumentFile);
            _db.EducationDocumentsData.Update(educationDocumentForm);
            await _db.SaveChangesAsync();
        }

        public async Task DeleteEducationDocumentFile(Guid userId)
        {
            var educationDocumentFile = await _db.EducationDocumentsFiles
                .FirstOrDefaultAsync(
                    ed => ed.OwnerId == userId);

            var educationDocumentForm = await _db.EducationDocumentsData
                .FirstOrDefaultAsync(
                ed => ed.OwnerId == userId);

            if (educationDocumentFile == null)
            {
                throw new NotFoundException("У пользователя нет скана уровня образования");
            }

            educationDocumentForm.fileId = Guid.Empty;

            _db.EducationDocumentsFiles.Remove(educationDocumentFile);
            _db.EducationDocumentsData.Update(educationDocumentForm);
            await _db.SaveChangesAsync();
            File.Delete(educationDocumentFile.Path);
        }
    }
}
