﻿using Common.DTO.Auth;
using Common.DTO.Entrance;
using Common.DTO.Profile;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AdminPanel.BL.Serivces.Interface
{
    public interface IQueueSender
    {
        public Task<AuthResponseDTO> Login(LoginRequestDTO loginCreds);
        public Task<ProfileResponseDTO> GetProfile(Guid userId);
        public Task SendMessage<T>(T message, string topik);
        public Task<ApplicationsResponseDTO> GetApplications(ApplicationFiltersDTO applicationFilters);
    }
}
