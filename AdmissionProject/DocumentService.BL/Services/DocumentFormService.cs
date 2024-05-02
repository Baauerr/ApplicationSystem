﻿using AutoMapper;
using DocumentService.Common.DTO;
using DocumentService.Common.Interface;
using DocumentService.DAL;
using DocumentService.DAL.Entity;
using Exceptions.ExceptionTypes;
using Microsoft.EntityFrameworkCore;

namespace DocumentService.BL.Services
{
    public class DocumentFormService : IDocumentFormService
    {

        private readonly DocumentDbContext _db;
        private readonly IMapper _mapper;
        private readonly IRequestService _requestService;
        private readonly IFileService _fileService;

        public DocumentFormService(DocumentDbContext dbContext, IMapper mapper, IRequestService requestService, IFileService fileService)
        {
            _db = dbContext;
            _mapper = mapper;
            _requestService = requestService;
            _fileService = fileService;
        }

        public async Task AddEducationDocumentsInfo(EducationDocumentFormDTO educationDocumentDTO, Guid userId)
        {
            await ValidateEducationLevel(educationDocumentDTO.EducationLevelId);

            var educationDocument = _mapper.Map<EducationDocumentForm>(educationDocumentDTO);

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

        public async Task DeletePassportInfo(Guid userId)
        {
            var currentPassport = await _db.PassportsData.FirstOrDefaultAsync(p => p.OwnerId == userId);

            if (currentPassport == null)
            {
                throw new BadRequestException("У этого пользователя уже есть паспорт");
            }
            else
            {
                _db.PassportsData.Remove(currentPassport);
            }
            await _fileService.DeletePassportFile(userId);
        }

        public async Task EditEducationDocumentsInfo(EducationDocumentFormDTO educationDocumentDTO, Guid userId)
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

        public async Task DeleteEducationDocumentsInfo(DeleteEducationFormDTO educationDocumentDTO, Guid userId)
        {
            var educationDocument = await _db.EducationDocumentsData.FirstOrDefaultAsync(
                ed => ed.EducationLevelId == educationDocumentDTO.EducationLevelId &&
                ed.OwnerId == userId);

            if (educationDocument == null)
            {
                throw new NotFoundException("У пользователя нет документа о таком образовании");
            }

            _db.EducationDocumentsData.Remove(educationDocument);
            await _db.SaveChangesAsync();
            await _fileService.DeleteEducationDocumentFile(userId, educationDocumentDTO);
        }

        public async Task EditPassportInfo(PassportFormDTO passportDTO, Guid userId)
        {

            //ПРОТЕСТИРОВАТЬ

            var currentPassport = await _db.PassportsData.FirstOrDefaultAsync(p => p.OwnerId == userId);

            currentPassport = _mapper.Map<PassportForm>(passportDTO);
            _db.Update(currentPassport);
            await _db.SaveChangesAsync();
        }

        public async Task<List<GetEducationDocumentFormDTO>> GetEducationDocumentsInfo(Guid userId)
        {
            var documentsData = await _db.EducationDocumentsData
              .Where(ed => ed.OwnerId == userId)
              .ToListAsync();

            if (documentsData.Count == 0)
            {
                throw new NotFoundException("Пользователь ещё не добавил документы об образовании");
            }

            var educationDocumentDTOList = new List<GetEducationDocumentFormDTO>();

            foreach (var document in documentsData)
            {
                var educationDocumentDTO = _mapper.Map<GetEducationDocumentFormDTO>(documentsData);
                educationDocumentDTO.UserId = userId;
                educationDocumentDTOList.Add(educationDocumentDTO);
            }

            return educationDocumentDTOList;
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
