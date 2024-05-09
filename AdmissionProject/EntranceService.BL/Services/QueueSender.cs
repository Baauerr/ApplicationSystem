using Common.DTO;
using Common.DTO.Dictionary;
using Common.DTO.Profile;
using DocumentService.Common.DTO;
using EasyNetQ;
using EntranceService.Common.Interface;


namespace EntranceService.BL.Services
{
    public class QueueSender
    {

        private IBus _bus;
        public QueueSender() {
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        public async Task SendMessage<T>(T message)
        {
            await _bus.PubSub.PublishAsync(message);
        }

        public async Task<ProgramResponseDTO> GetAllPrograms(Guid userId)
        {
            var programs = await _bus.Rpc.RequestAsync<GetPrograms, ProgramResponseDTO>(new GetPrograms { UserId = userId});

            return programs;
        }

        public async Task<EducationLevelResponseDTO> GetAllEducationLevels(Guid userId)
        {
            var educationLevels = await _bus.Rpc.RequestAsync<GetEducationLevels, EducationLevelResponseDTO>(new GetEducationLevels { UserId = userId }, x => x.WithQueueName("huy"));

            return educationLevels;
        }

        public async Task<PassportFormDTO> GetUserPassport(Guid userId)
        {
            var passportInfo = await _bus.Rpc.RequestAsync<GetPassportInfo, GetPassportFormDTO>(new GetPassportInfo { UserId = userId });
            return passportInfo;
        }
    }
}
