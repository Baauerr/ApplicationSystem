﻿using AdminPanel.BL.Serivces.Interface;
using Common.Const;
using Common.DTO.Auth;
using Common.DTO.Dictionary;
using Common.DTO.Entrance;
using Common.DTO.Profile;
using Common.DTO.Document;
using EasyNetQ;
using EasyNetQ.DI;
using Common.DTO.User;

namespace AdminPanel.BL.Serivces.Impl
{
    public class QueueSender : IQueueSender
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

        public async Task<PassportFormDTO> GetPassportForm(Guid userId)
        {
            var passportForm = await _bus.Rpc.RequestAsync<Guid, PassportFormDTO>
                                (userId,
                                x => x.WithQueueName(QueueConst.GetPassportFormQueue));

            return passportForm;
        }

        public async Task<GetEducationDocumentFormDTO> GetEducationDocumentForm(Guid userId)
        {
            var educationDocumentForm = await _bus.Rpc.RequestAsync<Guid, GetEducationDocumentFormDTO>
                                (userId,
                                x => x.WithQueueName(QueueConst.GetEducationDocumentsFormsQueue));

            return educationDocumentForm;
        }

        public async Task<EducationLevelResponseDTO> GetAllEducationLevels()
        {
            var educationLevels = await _bus.Rpc.RequestAsync<Guid, EducationLevelResponseDTO>
                (Guid.Empty, x => x.WithQueueName(QueueConst.GetEducationLevelsQueue));

            return educationLevels;
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

        public async Task<ManagersListDTO> GetAllManagers()
        {
            var managers = await _bus.Rpc.RequestAsync<Guid, ManagersListDTO>
                (Guid.Empty, x => x.WithQueueName(QueueConst.GetAllManagersQueue));

            return managers;
        }

        public async Task SendMessage<T>(T message, string topik)
        {
            await _bus.PubSub.PublishAsync(message, topik);
        }

        public async Task<FacultiesResponseDTO> GetAllFaculties()
        {
            var faculties = await _bus.Rpc.RequestAsync<Guid, FacultiesResponseDTO>
                (Guid.Empty, x => x.WithQueueName(QueueConst.GetAllFacultiesQueue));

            return faculties;
        }

        public async Task<ProgramResponseDTO> GetAllPrograms()
        {
            var filters = new ProgramsFilterDTO();
            filters.pageSize = 5000;

            var applicationsResponse = await _bus.Rpc.RequestAsync<ProgramsFilterDTO, ProgramResponseDTO>
                (filters, x => x.WithQueueName(QueueConst.GetProgramsQueue));

            return applicationsResponse;
        }

        public async Task<AllImportHistoryDTO> GetImportHistory()
        {
            var importHistory = await _bus.Rpc.RequestAsync<Guid, AllImportHistoryDTO>
                (Guid.Empty, x => x.WithQueueName(QueueConst.GetImportHistoryQueue));

            return importHistory;
        }

        public async Task<GetApplicationPrograms> GetApplicationPrograms(Guid userId)
        {
            var programs = await _bus.Rpc.RequestAsync<Guid, GetApplicationPrograms>
                (userId, x => x.WithQueueName(QueueConst.GetApplicationProgramsQueue));

            return programs;
        }

        public async Task<GetAllUsersDTO> GetAllUsers(UsersFilterDTO filter)
        {
            var users = await _bus.Rpc.RequestAsync<UsersFilterDTO, GetAllUsersDTO>
                (filter, x => x.WithQueueName(QueueConst.GetAllUsersQueue));

            return users;
        }

        public async Task<GetApplicationManagerId> GetApplicationManagerId(Guid userId)
        {
            var users = await _bus.Rpc.RequestAsync<Guid, GetApplicationManagerId>
                (userId, x => x.WithQueueName(QueueConst.GetApplicationManagerIdQueue));

            return users;
        }
    }
}

