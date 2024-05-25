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

            return applcationDTO;
        }
        public async Task SyncUserDataInApplication(UpdateUserDataDTO updateUserDataDTO)
        {
            var userApplication = _db.Applications.FirstOrDefault(ap => ap.OwnerId == updateUserDataDTO.UserId);
            var manager = await _db.Managers.FirstOrDefaultAsync(ap => ap.Id == updateUserDataDTO.UserId);

            if (userApplication != null)
            {
                userApplication.OwnerName = updateUserDataDTO.NewUserName;
                userApplication.OwnerEmail = updateUserDataDTO.NewEmail;
                _db.Applications.Update(userApplication);
            }
            if (manager != null) {

                manager.FullName = updateUserDataDTO.NewUserName;
                manager.Email = updateUserDataDTO.NewEmail;

                _db.Managers.Update(manager);
            }
            
            await _db.SaveChangesAsync();
        }
    }
}
