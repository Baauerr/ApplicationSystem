using AdminPanel.BL.Serivces.Interface;
using AdminPanel.Models;
using Common.Const;
using Common.DTO.Entrance;
using Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;


namespace AdminPanel.Controllers
{
  //  [Authorize(Roles = "Manager")]
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
        public async Task<IActionResult> Applications()
        {
                var emptyFilters = new ApplicationFiltersDTO();

                var applicationsModel = await GetApplications(emptyFilters);

                return View(applicationsModel);
        }

        [HttpPost]
        public async Task<IActionResult> Applications(ApplicationFiltersDTO filters)
        {
            try
            {
                var applicationsModel = await GetApplications(filters);

                return PartialView("_ApplicationsList", applicationsModel);
            }
            catch
            {
                ModelState.AddModelError("", "Ошибка при получении заявок");
                return BadRequest(new { success = false, message = "Ошибка при получении заявок" });
            }
        }

        public async Task<ApplicationsViewModel> GetApplications(ApplicationFiltersDTO filters)
        {

                var applications = await _queueSender.GetApplications(filters);

                var token = HttpContext.Request.Cookies["AccessToken"];
                var userId = _tokenHelper.GetUserIdFromToken(token);

                var viewModel = new ApplicationsViewModel
                {
                    ApplicationsResponse = applications,
                    Filters = filters,
                    myId = userId
                };

                return viewModel;            
        }

        [HttpPost]
        [Authorize]
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
        [Authorize]
        public async Task<IActionResult> RefuseApplication(Guid applicationId)
        {

            var token = HttpContext.Request.Cookies["AccessToken"];
            var userId = _tokenHelper.GetUserIdFromToken(token);

            var refuseApplication = new RefuseApplication
            {
                ManagerId = userId,
                ApplicationId = applicationId
            };

            await _queueSender.SendMessage(refuseApplication, QueueConst.RemoveApplicationManagerQueue);

            return RedirectToAction("Applications");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> TakeApplication(Guid applicationId)
        {
            var token = HttpContext.Request.Cookies["AccessToken"];
            var userId = _tokenHelper.GetUserIdFromToken(token);

            var takeApplication = new TakeApplication
            {
                ManagerId = userId, 
                ApplicationId = applicationId
            };

            await _queueSender.SendMessage(takeApplication, QueueConst.SetManagerQueue);

            return RedirectToAction("Applications");
        }

    }
}
