using Common.Const;
using Common.DTO.Dictionary;
using Common.DTO.Profile;
using DocumentService.Common.DTO;
using EasyNetQ;


namespace EntranceService.BL.Services
{
    public class QueueSender
    {

        private IBus _bus;
        public QueueSender() {
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        public async Task SendMessage<T>(T message, string topik)
        {
            await _bus.PubSub.PublishAsync(message, topik);
        }

        public async Task<ProgramResponseDTO> GetAllPrograms(Guid userId)
        {
            var programs = await _bus.Rpc.RequestAsync<Guid, ProgramResponseDTO>(userId, x => x.WithQueueName(QueueConst.GetProgramsQueue));

            return programs;
        }

        public async Task<EducationLevelResponseDTO> GetAllEducationLevels(Guid userId)
        {
            var educationLevels = await _bus.Rpc.RequestAsync<Guid, EducationLevelResponseDTO>
                (userId, x => x.WithQueueName(QueueConst.GetEducationLevelsQueue));

            return educationLevels;
        }

        public async Task<PassportFormDTO> GetUserPassport(Guid userId)
        {
            var passportInfo = await _bus.Rpc.RequestAsync<Guid, GetPassportFormDTO>
                (userId, x => x.WithQueueName(QueueConst.GetPassportFormQueue));
            return passportInfo;
        }
        public async Task<GetEducationDocumentFormDTO> GetUserEducationDocument(Guid userId)
        {
            var educationDocument = await _bus.Rpc.RequestAsync<Guid, GetEducationDocumentFormDTO>
                (userId, x => x.WithQueueName(QueueConst.GetEducationDocumentsFormsQueue));
            return educationDocument;
        }
        public async Task<ProfileResponseDTO> GetProfileInfo(Guid userId)
        {
            var userInfo = await _bus.Rpc.RequestAsync<Guid, ProfileResponseDTO>
                (userId, x => x.WithQueueName(QueueConst.GetProfileInfoQueue));

            return userInfo;
        }
    }
}
