using AutoMapper;
using Common.DTO.Entrance;
using EasyNetQ;
using EntranceService.Common.DTO;
using EntranceService.Common.Interface;
using EntranceService.DAL;
using Exceptions.ExceptionTypes;
using Microsoft.EntityFrameworkCore;
using NotificationService.DTO;

namespace EntranceService.BL.Services
{
    public class EntrantService : IEntrantService
    {

        private readonly EntranceDbContext _db;
        private readonly IMapper _mapper;
        private readonly IBus _bus;

        public EntrantService(EntranceDbContext db, IMapper mapper)
        {
            _db = db;
            _mapper = mapper;
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        public async Task EditApplicationsInfo(EditApplicationDTO newApplicationInfo, Guid userId)
        {
            var application = await _db.Applications.FirstOrDefaultAsync(ap => ap.Id == newApplicationInfo.applicationId);

            if (application == null)
            {
                throw new NotFoundException("Такой заявки не существует");
            }

            application.Citizenship = newApplicationInfo.Citizenship;
            _db.Update(application);
            await _db.SaveChangesAsync();
        }

        public async Task<GetApplicationDTO> GetApplicationInfo(Guid applicationId)
        {
            var application = await _db.Applications.FirstOrDefaultAsync(ap => ap.Id == applicationId);
            var applicationPrograms = _db.ApplicationsPrograms
                .Where(ap => ap.ApplicationId == applicationId)
                .ToList();  

            if (application == null)
            {
                throw new NotFoundException("Заявки с таким id не существует");
            }

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
        public async Task SyncNameInApplication(UpdateUserDataDTO updateUserDataDTO)
        {
            var application = _db.Applications.FirstOrDefault(ap => ap.OwnerId == updateUserDataDTO.UserId);
            if (application == null)
            {
                throw new NotFoundException("У пользователя нет заявления");
            }
            application.OwnerName = updateUserDataDTO.NewUserName;
            _db.Applications.Update(application);
            await _db.SaveChangesAsync();
        }
    }
}
