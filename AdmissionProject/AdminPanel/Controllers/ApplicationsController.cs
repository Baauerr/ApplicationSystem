using AdminPanel.BL.Serivces.Interface;
using AdminPanel.Models;
using Common.Const;
using Common.DTO.Entrance;
using Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using System;

namespace AdminPanel.Controllers
{
    public class ApplicationsController : Controller
    {

        private readonly IQueueSender _queueSender;
        private readonly ITokenHelper _tokenHelper;
        public ApplicationsController(IQueueSender queueSender, ITokenHelper token)
        {
            _queueSender = queueSender;
            _tokenHelper = token;
        }

        [HttpGet]
        public async Task<IActionResult> Applications(ApplicationFiltersDTO filters)
        {
            try
            {
                var applications = await _queueSender.GetApplications(filters); 
                var email = HttpContext.Session.GetString("Email");
                ViewBag.Email = email;
                return View(applications);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(GetApplicationStatus data)
        {
            var changeStatus = new ChangeApplicationStatusDTO
            {
                ApplcationStatus = data.status,
                ApplicationId =  data.applicationId
            };

            await _queueSender.SendMessage(changeStatus, QueueConst.ChangeApplicationStatusQueue);

            return RedirectToAction("Applications");
        }

        [HttpPost]
        public async Task<IActionResult> RefuseApplication(Guid applicationId)
        {

            var token = HttpContext.Session.GetString("AccessToken");
            var userId = _tokenHelper.GetUserIdFromToken(token);

            var refuseApplication = new ApplicationManager
            {
                ManagerId = userId,
                ApplicationId = applicationId
            };

            await _queueSender.SendMessage(refuseApplication, QueueConst.RemoveApplicationManagerQueue);

            return RedirectToAction("Applications");
        }

        [HttpPost]
        public async Task<IActionResult> TakeApplication(Guid applicationId)
        {
            var token = HttpContext.Session.GetString("AccessToken");
            var userId = _tokenHelper.GetUserIdFromToken(token);

            var takeApplication = new ApplicationManager
            {
                ManagerId = userId, 
                ApplicationId = applicationId
            };

            await _queueSender.SendMessage(takeApplication, QueueConst.SetManagerQueue);

            return RedirectToAction("Applications");
        }

    }
}
