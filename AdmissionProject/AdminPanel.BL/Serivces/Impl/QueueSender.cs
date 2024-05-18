using AdminPanel.BL.Serivces.Interface;
using Common.Const;
using Common.DTO.Auth;
using Common.DTO.Entrance;
using Common.DTO.Profile;
using EasyNetQ;
using EasyNetQ.DI;

namespace AdminPanel.BL.Serivces.Impl
{
    public class QueueSender: IQueueSender
    {
        private IBus _bus;
        public QueueSender()
        {
            _bus = RabbitHutch.CreateBus("host=localhost", x => x.Register<ErrorMessageHandler>(_ => new ErrorMessageHandler()));
        }

        public async Task<ProfileResponseDTO> GetProfile(Guid userId)
        {
            var profileInfo = await _bus.Rpc.RequestAsync<Guid, ProfileResponseDTO>
                                (userId,
                                x => x.WithQueueName(QueueConst.GetProfileInfoQueue));

            return profileInfo;
        }

        public async Task<AuthResponseDTO> Login(LoginRequestDTO loginCreds)
        {
                var loginResponse = await _bus.Rpc.RequestAsync<LoginRequestDTO, AuthResponseDTO>
                                (loginCreds, x => x.WithQueueName(QueueConst.LoginQueue));

                return loginResponse;
        }

        public async Task<ApplicationsResponseDTO> GetApplications(ApplicationFiltersDTO applicationFilters)
        {
            var applicationsResponse = await _bus.Rpc.RequestAsync<ApplicationFiltersDTO, ApplicationsResponseDTO>
                (applicationFilters, x => x.WithQueueName(QueueConst.GetApplicationsQueue));

            return applicationsResponse;
        }

        public async Task SendMessage<T>(T message, string topik)
        {
            await _bus.PubSub.PublishAsync(message, topik);
        }

    }
}
