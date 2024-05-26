using AdminPanel.BL.Serivces.Interface;
using AdminPanel.Models;
using Common.Const;
using Common.DTO.User;
using Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static NuGet.Packaging.PackagingConstants;

namespace AdminPanel.Controllers
{
    public class UsersController : Controller
    {
        private readonly IQueueSender _queueSender;
        private readonly ITokenHelper _tokenHelper;

        public UsersController(IQueueSender queueSender, ITokenHelper token)
        {
            _queueSender = queueSender;
            _tokenHelper = token;
        }

        [HttpGet]
        public async Task<IActionResult> Users()
        {
            var filters = new UsersFilterDTO();

            var users = await _queueSender.GetAllUsers(filters);

            var userView = new UsersViewModel
            {
                Filters = filters,
                Users = users
            };

            return View(userView);
        }

        [HttpPost]
        public async Task<IActionResult> Users(UsersFilterDTO filters)
        {
            var users = await _queueSender.GetAllUsers(filters);

            var userView = new UsersViewModel
            {
                Filters = filters,
                Users = users
            };

            return View(userView);
        }

        [HttpPost]
        public async Task RemoveRole(DeleteUserRoleDTO data)
        {
            await _queueSender.SendMessage(data, QueueConst.RemoveUserRoleQueue);
        }

        [HttpPost]
        public async Task SetRole(UserRoleActionDTO data)
        {
            await _queueSender.SendMessage(data, QueueConst.SetRoleQueue);

        }
    }
}
