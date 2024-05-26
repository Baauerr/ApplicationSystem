using Common.Const;
using Common.DTO.Dictionary;
using Common.DTO.Profile;
using Common.DTO.Document;
using EasyNetQ;
using Exceptions.ExceptionTypes;

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

        public async Task<ProgramResponseDTO> GetAllPrograms(ProgramsFilterDTO programsFilter, Guid userId)
        {

            var programs = await _bus.Rpc.RequestAsync<ProgramsFilterDTO, ProgramResponseDTO>(programsFilter, x => x.WithQueueName(QueueConst.GetProgramsQueue));

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
            try
            {
                var passportInfo = await _bus.Rpc.RequestAsync<Guid, GetPassportFormDTO>
                (userId, x => x.WithQueueName(QueueConst.GetPassportFormQueue));
                return passportInfo;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.StackTrace);
                throw new BadRequestException(ex.Message);
            }            
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
