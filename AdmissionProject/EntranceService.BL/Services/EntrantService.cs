using AutoMapper;
using Common.DTO.Entrance;
using EntranceService.Common.Interface;
using EntranceService.DAL;
using Exceptions.ExceptionTypes;
using Microsoft.EntityFrameworkCore;

namespace EntranceService.BL.Services
{
    public class EntrantService : IEntrantService
    {

        private readonly EntranceDbContext _db;
        private readonly IMapper _mapper;

        public EntrantService(EntranceDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
        }

        public async Task EditApplicationsInfo(EditApplicationDTO newApplicationInfo, Guid userId)
        {
            var application = await _db.Applications.FirstOrDefaultAsync(ap => ap.Id == newApplicationInfo.applicationId);
            
            if (application == null)
            {
                throw new NotFoundException("Такой заявки не существует");
            }

            if (application.OwnerId != userId || application.ManagerId != userId)
            {
                throw new ForbiddenException("Редактировать заявку может только владелец или менеджер этой заявки");
            }

            application.Citizenship = newApplicationInfo.Citizenship;
            _db.Update(application);
            await _db.SaveChangesAsync();
        }

        public async Task<GetApplicationDTO> GetApplicationInfo(Guid userId)
        {
            var application = await _db.Applications.FirstOrDefaultAsync(ap => ap.OwnerId == userId);

            if (application == null)
            {
                throw new NotFoundException("Заявки с таким id не существует");
            }

            var applicationPrograms = _db.ApplicationsPrograms
                .Where(ap => ap.ApplicationId == application.Id);

            var applcationDTO = _mapper.Map<GetApplicationDTO>(application);

            var programsDTO = new List<GetProgramDTO>();

            foreach (var program in applicationPrograms)
            {
                var programDTO = _mapper.Map<GetProgramDTO>(program);
                programsDTO.Add(programDTO);
            }

            applcationDTO.Programs = programsDTO;

            return applcationDTO;
        }
        public async Task SyncUserDataInApplication(UpdateUserDataDTO updateUserDataDTO)
        {
            var userApplication = _db.Applications.FirstOrDefault(ap => ap.OwnerId == updateUserDataDTO.UserId);
            var managerApplication = _db.Applications.Where(ap => ap.ManagerId == updateUserDataDTO.UserId);

            if (userApplication != null)
            {
                userApplication.OwnerName = updateUserDataDTO.NewUserName;
                userApplication.OwnerEmail = updateUserDataDTO.NewEmail;
                _db.Applications.Update(userApplication);
            }
            if (managerApplication.Any()) {
                foreach (var application in managerApplication)
                {
                 //    application.ManagerEmail = updateUserDataDTO.NewEmail;
                  //  application.ManagerFullName = updateUserDataDTO.NewUserName;
                }
                _db.Applications.UpdateRange(managerApplication);
            }
            
            await _db.SaveChangesAsync();
        }
    }
}
