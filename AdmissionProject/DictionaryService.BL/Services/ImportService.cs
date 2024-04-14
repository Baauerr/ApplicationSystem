using AutoMapper;
using DictionaryService.Common.DTO;
using DictionaryService.Common.Interfaces;
using DictionaryService.DAL;
using DictionaryService.DAL.Entities;
using DictionaryService.DAL.Enum;
using Exceptions.ExceptionTypes;
using Microsoft.EntityFrameworkCore;
using System;
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

        public async Task ImportDictionary(OperationType operationType)
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
                UserId = Guid.NewGuid(),
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

            Console.WriteLine(response.Content);

            if (response.IsSuccessStatusCode)
            {
                var currentDocumnetTypes = await _db.DocumentTypes.ToListAsync();
                List<DocumentType> documentTypesList = new List<DocumentType>();
                List<NextEducationLevel> nextEducationLevelList = new List<NextEducationLevel>();
                List<DocumentTypeDTO> documentTypes = await response.Content.ReadAsAsync<List<DocumentTypeDTO>>();
                foreach (var docuementType in documentTypes)
                {
                    if (!currentDocumnetTypes.Any(doc => doc.Id == docuementType.Id)) {
                        var newDocumentType = _mapper.Map<DocumentType>(docuementType);
                        newDocumentType.CreateTime = newDocumentType.CreateTime.ToUniversalTime();

                        foreach (var educationLevel in docuementType.NextEducationLevels)
                        {
                            var newNextEducationLevel = new NextEducationLevel
                            {
                                DocumentTypes = newDocumentType,
                                DocumentTypeId = newDocumentType.Id, 
                                EducationLevelId = educationLevel.Id,
                                EducationLevels = educationLevel
                            };
                            newDocumentType.NextEducationLevels.Add(newNextEducationLevel);
                        }
                        documentTypesList.Add(newDocumentType);
                    }                
                }
                await _db.DocumentTypes.AddRangeAsync(documentTypesList);
           
                return ImportStatus.Success;
            }
            else
            {
                return ImportStatus.Failed;
            }  
        }

        private async Task<ImportStatus> ImportEducationLevels()
        {
        
            HttpResponseMessage response = await _httpClient.GetAsync(_baseUrl + "education_levels");

            if (response.IsSuccessStatusCode)
            {
                var currentEducationLevels = await _db.EducationLevels.ToListAsync();
                List<EducationLevel> educationLevelsList = new List<EducationLevel>();
                List<EducationLevel> educationLevels = await response.Content.ReadAsAsync<List<EducationLevel>>();
            
                foreach (var educationLevel in educationLevels)
                {
                    if (!currentEducationLevels.Any(level => level.Id == educationLevel.Id))
                    {
                        educationLevelsList.Add(educationLevel);
                    } 
                }
                await _db.EducationLevels.AddRangeAsync(educationLevelsList);
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
                List<Faculty> facultiesList = new List<Faculty>();
                List<Faculty> faculties = await response.Content.ReadAsAsync<List<Faculty>>();

                foreach (var faculty in faculties)
                {
                    if (!currentFaculties.Any(faculty => faculty.Id == faculty.Id))
                    {
                        faculty.CreateTime = faculty.CreateTime.ToUniversalTime();
                        facultiesList.Add(faculty);
                    }        
                }
                await _db.Faculties.AddRangeAsync(facultiesList);
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
                List<Program> programsList = new List<Program>();
                ProgramResponseDTO programs = await response.Content.ReadAsAsync<ProgramResponseDTO>();
                foreach (var program in programs.programs)
                {
                    if (!currentPrograms.Any(program => program.Id == program.Id))
                    {
                        var newProgram = _mapper.Map<Program>(program);
                        newProgram.CreateTime = program.CreateTime.ToUniversalTime();
                        programsList.Add(newProgram);
                    }
                }
                await _db.Programs.AddRangeAsync(programsList);
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
