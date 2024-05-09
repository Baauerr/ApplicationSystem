using AutoMapper;
using Common.DTO.Dictionary;
using DictionaryService.Common.Interfaces;
using DictionaryService.DAL;
using DictionaryService.DAL.Entities;
using DictionaryService.DAL.Enum;
using Exceptions.ExceptionTypes;
using Microsoft.EntityFrameworkCore;
using System.Net;
using System.Net.Http.Headers;
using System.Text;

namespace DictionaryService.BL.Services
{
    public class ImportService : IImportService
    {
        private readonly HttpClient _httpClient;
        private readonly IMapper _mapper;
        private readonly string _username = "student";
        private readonly string _password = "ny6gQnyn4ecbBrP9l1Fz";
        private readonly string _baseUrl = "https://1c-mockup.kreosoft.space/api/dictionary/";
        private readonly DictionaryDbContext _db;
        public ImportService(DictionaryDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _httpClient = CreateHttpClient(_username, _password);
        }
        private HttpClient CreateHttpClient(string username, string password)
        {
            HttpClientHandler handler = new HttpClientHandler();
            handler.Credentials = new NetworkCredential(username, password);

            HttpClient client = new HttpClient(handler);

 
            string credentials = Convert.ToBase64String(Encoding.ASCII.GetBytes($"{username}:{password}"));
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Basic", credentials);

            return client;
        }

        public async Task ImportDictionary(OperationType operationType, Guid userId)
        {
            var operationResult = ImportStatus.Failed;

            switch (operationType)
            {
                case OperationType.Faculty:
                    operationResult = await ImportFaculties();
                    break;
                case OperationType.DocumentType:
                    operationResult = await ImportDocumentTypes();
                    break;
                case OperationType.Program:
                    operationResult = await ImportPrograms();
                    break;
                case OperationType.EducationLevel:
                    operationResult = await ImportEducationLevels();
                    break;
            }

            var historyElement = new ImportHistory
            {
                UserId = userId,
                ImportStatus = operationResult,
                OperationType = operationType,
                OperationTime = DateTime.UtcNow,
            };

            await _db.ImportHistory.AddAsync(historyElement);

            await _db.SaveChangesAsync();
        }

        private async Task<ImportStatus> ImportDocumentTypes()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + "document_types");

            if (response.IsSuccessStatusCode)
            {
                var currentDocumentTypes = await _db.DocumentTypes.ToListAsync();
                var newDocumentTypes = await response.Content.ReadAsAsync<List<DocumentTypeDTO>>();

                var toDelete = currentDocumentTypes
                    .Where(doc => !newDocumentTypes.Any(newDoc => newDoc.Id == doc.Id)) 
                    .ToList();

                var toAdd = newDocumentTypes
                    .Where(doc => !currentDocumentTypes.Any(curDoc => curDoc.Id == doc.Id))
                    .ToList();
                
                var toUpdate = currentDocumentTypes
                    .Where(doc => !newDocumentTypes.Any(newDoc => newDoc.Id == doc.Id)) 
                    .ToList();

                _db.DocumentTypes.RemoveRange(toDelete);

                await DeleteNextEducationLevels(toDelete);
               
                foreach(var documentTypeDTO in toAdd)
                {
                    var documentType = _mapper.Map<DocumentType>(documentTypeDTO);
                    foreach(var nextEducationLevelDTO in documentTypeDTO.NextEducationLevels)
                    {
                        var newNextEducationLevel = new NextEducationLevel
                        {
                            DocumentTypeId = documentTypeDTO.Id,
                            EducationLevelId = nextEducationLevelDTO.Id,
                            EducationLevelName = nextEducationLevelDTO.Name,
                        };
                        await _db.NextEducationLevelDocuments.AddAsync(newNextEducationLevel);
                    }
                    await _db.DocumentTypes.AddAsync(documentType);
                }

                foreach (var documentType in toUpdate)
                {

                    var documentTypeDTO = newDocumentTypes
                        .Find(ndt => ndt.Id == documentType.Id);


                    documentType.EducationLevelId = documentTypeDTO.EducationLevel.Id;
                    documentType.Id = documentTypeDTO.Id;
                    documentType.Name = documentTypeDTO.Name;
                    documentType.CreateTime = documentTypeDTO.CreateTime;

                    _db.DocumentTypes.Update(documentType);

                    var nextEducationLevelList = new List<NextEducationLevel>();

                        foreach (var educationLevel in documentTypeDTO.NextEducationLevels)
                        {
                            var newNextEducationLevel = new NextEducationLevel
                            {
                                DocumentTypeId = documentType.Id, 
                                EducationLevelId = educationLevel.Id,
                                EducationLevelName = educationLevel.Name,
                            };
                        _db.NextEducationLevelDocuments.Update(newNextEducationLevel);
                        }   
                }
                return ImportStatus.Success;
            }
            else
            {
                return ImportStatus.Failed;
            }  
        }

        private async Task DeleteNextEducationLevels(List<DocumentType> documentTypes)
        {
            foreach(var documentType in documentTypes)
            {
                var nextLevels = await _db.NextEducationLevelDocuments
                    .Where(nel => nel.DocumentTypeId == documentType.Id)
                    .ToListAsync();

                _db.NextEducationLevelDocuments.RemoveRange(nextLevels);
            }
        }

        private async Task<ImportStatus> ImportEducationLevels()
        {       
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + "education_levels");

            if (response.IsSuccessStatusCode)
            {
                var currentEducationLevels = await _db.EducationLevels.ToListAsync();
                List<EducationLevel> newEducationLevels = await response.Content.ReadAsAsync<List<EducationLevel>>();

                var toDelete = currentEducationLevels
                    .Where(doc => !newEducationLevels.Any(newDoc => newDoc.Id == doc.Id))
                    .ToList();

                var toAdd = newEducationLevels
                    .Where(doc => !currentEducationLevels.Any(curDoc => curDoc.Id == doc.Id))
                    .ToList();

                var toUpdate = currentEducationLevels
                    .Where(doc => !newEducationLevels.Any(newDoc => newDoc.Id == doc.Id))
                    .ToList();

                await _db.EducationLevels.AddRangeAsync(toAdd);
                _db.EducationLevels.RemoveRange(toDelete);


                foreach (var educationLevel in toUpdate)
                {
                    var educationLevelDTO = newEducationLevels.Find(el => el.Id == educationLevel.Id);
                    educationLevel.Id = educationLevelDTO.Id;
                    educationLevel.Name = educationLevelDTO.Name;

                    _db.EducationLevels.Update(educationLevel);
                }

                await _db.SaveChangesAsync();
                return ImportStatus.Success;
            }
            else
            {
                return ImportStatus.Failed;
            }
        }

        private async Task<ImportStatus> ImportFaculties()
        {

            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + "faculties");
            if (response.IsSuccessStatusCode)
            {
                var currentFaculties = await _db.Faculties.ToListAsync();
                List<Faculty> newFaculties = await response.Content.ReadAsAsync<List<Faculty>>();

                var toDelete = currentFaculties
                    .Where(doc => !newFaculties.Any(newDoc => newDoc.Id == doc.Id))
                    .ToList();

                var toAdd = newFaculties
                    .Where(doc => !currentFaculties.Any(curDoc => curDoc.Id == doc.Id))
                    .ToList();

                var toUpdate = currentFaculties
                    .Where(doc => !newFaculties.Any(newDoc => newDoc.Id == doc.Id))
                    .ToList();

                await _db.Faculties.AddRangeAsync(toAdd);
                _db.Faculties.RemoveRange(toDelete);

                foreach (var faculty in toUpdate)
                {

                    var facultyDTO = newFaculties.Find(f => f.Id == faculty.Id);

                    faculty.Name = facultyDTO.Name;
                    faculty.Id = facultyDTO.Id;
                    faculty.CreateTime = facultyDTO.CreateTime;                  

                    _db.Faculties.Update(faculty);
                }  
                
                _db.SaveChangesAsync();

                return ImportStatus.Success;
            }
            else
            {
                return ImportStatus.Failed;
            }
        }

        private async Task<ImportStatus> ImportPrograms()
        {
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + $"programs?page={1}&size={1000000}");
            if (response.IsSuccessStatusCode)
            {
                var currentPrograms = await _db.Programs.ToListAsync();
                ProgramResponseDTO newPrograms = await response.Content.ReadAsAsync<ProgramResponseDTO>();

                var toDelete = currentPrograms
                    .Where(doc => !newPrograms.programs.Any(newDoc => newDoc.Id == doc.Id))
                    .ToList();

                var toAdd = newPrograms.programs
                    .Where(doc => !currentPrograms.Any(curDoc => curDoc.Id == doc.Id))
                    .ToList();

                var toUpdate = currentPrograms
                    .Where(doc => !newPrograms.programs.Any(newDoc => newDoc.Id == doc.Id))
                    .ToList();

                _db.Programs.RemoveRange(toDelete);

                foreach (var program in toUpdate)
                {
                    var programDTO = newPrograms.programs.Find(p => p.Id == program.Id);
                    program.Id = programDTO.Id;
                    program.FacultyId = programDTO.Faculty.Id;
                    program.Name = programDTO.Name;
                    program.CreateTime = programDTO.CreateTime;
                    program.Code = programDTO.Code;
                    program.EducationLevelId = programDTO.EducationLevel.Id;
                    program.Language = programDTO.Language;
                    program.FacultyId = programDTO.Faculty.Id;
                
                        
                    _db.Programs.Update(program);
                }

                foreach (var programDTO in toAdd)
                {
                    
                    var program = _mapper.Map<Program>(programDTO);

                    _db.Programs.Add(program);
                }

                await _db.SaveChangesAsync();

                return ImportStatus.Success;
            }
            else
            {
                return ImportStatus.Failed;
            }
        }
        public async Task<ImportStatus> GetImportStatus(Guid importId)
        {
            var import = await _db.ImportHistory
                .Where(import => import.Id == importId)
                .FirstOrDefaultAsync();

            if (import == null)
            {
                throw new NotFoundException("Такого импорта нет");
            }   
            return import.ImportStatus;
        }
        public async Task<List<ImportHistory>> GetImportHistory()
        {
            var import = await _db.ImportHistory.ToListAsync();

            if (import == null)
            {
                throw new NotFoundException("Такого импорта нет");
            }
            return import;
        }
    }
}
