using AutoMapper;
using Common.DTO.Dictionary;
using DocumentService.Common.Interface;
using DocumentService.DAL;
using DocumentService.DAL.Entity;
using Exceptions.ExceptionTypes;
using Microsoft.EntityFrameworkCore;
using Common.DTO.Document;

namespace DocumentService.BL.Services
{
    public class DocumentFormService : IDocumentFormService
    {

        private readonly DocumentDbContext _db;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly QueueSender _queueSender;
        

        public DocumentFormService(DocumentDbContext dbContext, IMapper mapper, IFileService fileService, QueueSender queueSender)
        {
            _db = dbContext;
            _mapper = mapper;
            _fileService = fileService;
            _queueSender = queueSender;
        }

        public async Task AddEducationDocumentInfo(EducationDocumentFormDTO educationDocumentDTO, Guid userId)
        {
            var educationLevels = await GetAllEducationLevels();

            await ValidateEducationLevel(educationDocumentDTO.EducationLevelId, educationLevels);

            var educationDocument = await _db.EducationDocumentsData.FirstOrDefaultAsync(ed => ed.OwnerId == userId);

            if (educationDocument != null)
            {
                throw new BadRequestException("У пользователя уже есть документ об уровне образования");
            }

            var currentEducationLevel = educationLevels.EducationLevel.FirstOrDefault(edl => edl.Id == educationDocumentDTO.EducationLevelId);

            var newEducationDocument = _mapper.Map<EducationDocumentForm>(educationDocumentDTO);

            newEducationDocument.OwnerId = userId;
            newEducationDocument.EducationLevelName = currentEducationLevel.Name;

            await _db.EducationDocumentsData.AddAsync(newEducationDocument);

            await _db.SaveChangesAsync();
        }

        private async Task ValidateEducationLevel(string educationLevelId, EducationLevelResponseDTO educationLevels)
        {
            

            var isEducationLevelExist = educationLevels.EducationLevel.Any(el => el.Id == educationLevelId);

            if (!isEducationLevelExist)
            {
                throw new NotFoundException("Такого уровня образования не существует");
            }
        }

        private async Task<EducationLevelResponseDTO> GetAllEducationLevels()
        {
            var educationLevels = _queueSender.GetEducationLevels();

            return educationLevels;
        }

        public async Task AddPassportInfo(PassportFormDTO passportDTO, Guid userId)
        {
            var currentPassport = await _db.PassportsData.FirstOrDefaultAsync(p => p.OwnerId == userId);

            if (currentPassport == null)
            {
                var passportData = _mapper.Map<PassportForm>(passportDTO);
                passportData.OwnerId = userId;
                Console.WriteLine(passportData.Number);
                await _db.PassportsData.AddAsync(passportData);
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new BadRequestException("У этого пользователя уже есть паспорт");
            }
        }
        public async Task EditEducationDocumentInfo(EducationDocumentFormDTO educationDocumentDTO, Guid userId)
        {

            var educationLevels = await GetAllEducationLevels();
            

            await ValidateEducationLevel(educationDocumentDTO.EducationLevelId, educationLevels);

            var educationLevelName = educationLevels.EducationLevel
                .FirstOrDefault(el => el.Id == educationDocumentDTO.EducationLevelId);

            var educationDocument = await _db.EducationDocumentsData.FirstOrDefaultAsync(ed => ed.OwnerId == userId);

            if (educationDocument == null)
            {
                throw new NotFoundException("У пользователя нет документа об уровне образовании");
            }

            educationDocument.Name = educationDocumentDTO.Name;
            educationDocument.EducationLevelId = educationDocumentDTO.EducationLevelId;
            educationDocument.EducationLevelName = educationLevelName.Name;

            _db.EducationDocumentsData.Update(educationDocument);
            await _db.SaveChangesAsync();
        }
        public async Task EditPassportInfo(PassportFormDTO passportDTO, Guid userId)
        {
            var currentPassport = await _db.PassportsData.FirstOrDefaultAsync(p => p.OwnerId == userId);

            if (currentPassport != null)
            {
                currentPassport.IssuePlace = passportDTO.IssuePlace;
                currentPassport.BirthPlace = passportDTO.BirthPlace;
                currentPassport.Number = passportDTO.Number;
                currentPassport.IssueDate = passportDTO.IssueDate;
                currentPassport.BirthPlace = passportDTO.BirthPlace;
                currentPassport.Series = passportDTO.Series;

                _db.Update(currentPassport);
                await _db.SaveChangesAsync();
            }
            else
            {
                throw new NotFoundException("Пользователь ещё не добавил паспорт");
            }
           
        }

        public async Task<GetEducationDocumentFormDTO> GetEducationDocumentInfo(Guid userId)
        {
            var documentData = await _db.EducationDocumentsData
              .FirstOrDefaultAsync(ed => ed.OwnerId == userId);

            if (documentData == null)
            {
                throw new NotFoundException("Пользователь ещё не добавил документ об образовании");
            }

            var educationDocumentDTOList = new List<GetEducationDocumentFormDTO>();

            var educationDocumentDTO = _mapper.Map<GetEducationDocumentFormDTO>(documentData);
            
            return educationDocumentDTO;
        }

        public async Task<GetPassportFormDTO> GetPassportInfo(Guid userId)
        {

            var passportInfo = await _db.PassportsData.FirstOrDefaultAsync(ed => ed.OwnerId == userId);

            if (passportInfo == null)
            {
                throw new NotFoundException("Пользователь ещё не добавил паспорт");
            }

            var passportDTO = _mapper.Map<GetPassportFormDTO>(passportInfo);
            passportDTO.UserId = userId;

            return passportDTO;
        } 
    }
}
