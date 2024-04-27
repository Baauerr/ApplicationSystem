using AutoMapper;
using EntranceService.Common.DTO;
using EntranceService.Common.Interface;
using EntranceService.DAL;
using EntranceService.DAL.Entity;
using Exceptions.ExceptionTypes;
using Microsoft.EntityFrameworkCore;

namespace EntranceService.BL.Services
{
    public class DocumentService : IDocumentService
    {

        private readonly EntranceDbContext _db;
        private readonly IMapper _mapper;
        private readonly IRequestService _requestService;

        public DocumentService(EntranceDbContext dbContext, IMapper mapper, IRequestService requestService) { 
            _db = dbContext;
            _mapper = mapper;
            _requestService = requestService;
        }

        public async Task AddEducationDocumentsInfo(EducationDocumentDTO educationDocumentDTO, Guid userId)
        {
            await ValidateEducationLevel(educationDocumentDTO.EducationLevelId);

            var educationDocument = _mapper.Map<EducationDocumentData>(educationDocumentDTO);

            await _db.EducationDocumentsData.AddAsync(educationDocument);

            await _db.SaveChangesAsync();
        }

        private async Task ValidateEducationLevel(string educationLevelId)
        {
            var educationLevels = await _requestService.GetEducationLevels();

            var isEducationLevelExist = educationLevels.EducationLevel.Any(el => el.Id == educationLevelId);

            if (!isEducationLevelExist)
            {
                throw new NotFoundException("Такого уровня образования не существует");
            }
        }

        public async Task AddPassportInfo(PassportInfoDTO passportDTO, Guid userId)
        {
            var currentPassport = await _db.PassportsData.FirstOrDefaultAsync(p => p.OwnerId == userId);

            if (currentPassport == null)
            {
                var passportData = _mapper.Map<PassportData>(passportDTO);
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

        public async Task EditEducationDocumentsInfo(EducationDocumentDTO educationDocumentDTO, Guid userId)
        {

            await ValidateEducationLevel(educationDocumentDTO.EducationLevelId);

            var educationDocument = await _db.EducationDocumentsData.FirstOrDefaultAsync(
                ed => ed.EducationLevelId == educationDocumentDTO.EducationLevelId &&
                ed.OwnerId == userId);

            if (educationDocument == null)
            {
                throw new NotFoundException("У пользователя нет документа о таком образовании");
            }

            educationDocument.Name = educationDocumentDTO.Name;
            educationDocument.EducationLevelId = educationDocumentDTO.EducationLevelId;

            _db.EducationDocumentsData.Update(educationDocument);
            await _db.SaveChangesAsync();
        }

        public async Task EditPassportInfo(PassportInfoDTO passportDTO, Guid userId)
        {

            //ПРОТЕСТИРОВАТЬ

            var currentPassport = await _db.PassportsData.FirstOrDefaultAsync(p => p.OwnerId == userId);

            currentPassport = _mapper.Map<PassportData>(passportDTO);
            _db.Update(currentPassport);
            await _db.SaveChangesAsync();
        }

        public async Task<List<EducationDocumentDTO>> GetEducationDocumentsInfo(Guid userId)
        {
              var documentsData = await _db.EducationDocumentsData
                .Where(ed => ed.OwnerId == userId)
                .ToListAsync();

            var educationDocumentDTOList = new List<EducationDocumentDTO>();

            foreach (var document in documentsData)
            {
                var educationDocumentDTO = _mapper.Map<EducationDocumentDTO>(documentsData);
                educationDocumentDTOList.Add(educationDocumentDTO);
            }

            return educationDocumentDTOList;
        }

        public async Task<PassportInfoDTO> GetPassportInfo(Guid userId)
        {
            var passportInfo = await _db.PassportsData.FirstOrDefaultAsync(ed => ed.OwnerId == userId);

            var passportDTO = _mapper.Map<PassportInfoDTO>(passportInfo);

            return passportDTO;
        }
    }
}
