using AdminPanel.BL.Serivces.Interface;
using AdminPanel.Models;
using Common.Const;
using Common.DTO.Entrance;
using Common.DTO.Profile;
using Common.DTO.User;
using Common.Enum;
using Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System.Runtime.InteropServices;

namespace AdminPanel.Controllers
{
    [Authorize(Roles = "MainManager")]
    public class ManagerController : Controller
    {
        private readonly IQueueSender _queueSender;
        private readonly ITokenHelper _tokenHelper;
        private readonly IMemoryCache _memoryCache;
        public ManagerController(IQueueSender queueSender, ITokenHelper token, IMemoryCache memoryCache)
        {
            _queueSender = queueSender;
            _tokenHelper = token;
            _memoryCache = memoryCache;
        }

        public async Task<IActionResult> ManagersPage()
        {

            var managersList = await _queueSender.GetAllManagers();

            ViewBag.Roles = _memoryCache.Get("roles") as List<Roles>;

            return View(managersList);    
        }
        [HttpGet]
        public async Task<IActionResult> ManagerProfile(Guid managerId)
        {

            var profileInfo = await _queueSender.GetProfile(managerId);

            var viewModel = new ManagerProfileViewModel
            {
                ManagerId = managerId,
                Profile = profileInfo
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ManagerProfile(EditManagerProfileDTO newProfileInfo)
        {

            var normalProfile = new ChangeProfileRequestDTO {
                Email = newProfileInfo.Email,
                BirthDate = newProfileInfo.BirthDate,
                Citizenship = newProfileInfo.Citizenship,
                FullName = newProfileInfo.FullName,
                Gender = newProfileInfo.Gender,
                PhoneNumber = newProfileInfo.PhoneNumber,
            };

                var editProfileDTO = new ChangeProfileRequestRPCDTO
                {
                    ProfileData = normalProfile,
                    UserId = newProfileInfo.UserId,
                };

                try
                {
                    await _queueSender.SendMessage(editProfileDTO, QueueConst.ChangeProfileQueue);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }

            var profileInfo = await _queueSender.GetProfile(newProfileInfo.UserId);

            return Json(profileInfo);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteManager(Guid managerId, Roles role)
        {
            var userRoleAction = new DeleteUserRoleDTO
            {
                Role = role,
                UserId = managerId
            };

            await _queueSender.SendMessage(userRoleAction, QueueConst.RemoveUserRoleQueue);

            var managersList = await _queueSender.GetAllManagers();
            return PartialView("_ManagersListPartial", managersList);
        }

        public IActionResult GiveRole(Roles role)
        {
            return View();
        }
    }
}
