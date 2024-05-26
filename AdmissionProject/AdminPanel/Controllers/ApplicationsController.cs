using AdminPanel.BL.Serivces.Interface;
using AdminPanel.Models;
using Common.Const;
using Common.DTO.Dictionary;
using Common.DTO.Entrance;
using Common.Enum;
using Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using System;


namespace AdminPanel.Controllers
{
    [Authorize(Roles = "Manager")]
    public class ApplicationsController : Controller
    {

        private readonly IQueueSender _queueSender;
        private readonly ITokenHelper _tokenHelper;
        private readonly IMemoryCache _memoryCache;
        public ApplicationsController(IQueueSender queueSender, ITokenHelper token, IMemoryCache memoryCache)
        {
            _queueSender = queueSender;
            _tokenHelper = token;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public async Task<IActionResult> Applications()
        {
            var emptyFilters = new ApplicationFiltersDTO();

            ViewBag.Roles = _memoryCache.Get("roles") as List<Roles>;

            var applicationsModel = await GetApplications(emptyFilters);

            var managers = await _queueSender.GetAllManagers();

            applicationsModel.Programs = await GetAllPrograms();
            applicationsModel.Faculties = await GetAllFaculties();
            applicationsModel.Managers = managers;

            return View(applicationsModel);
        }

        [HttpPost]
        public async Task<IActionResult> Applications(ApplicationFiltersDTO filters)
        {

            ViewBag.Roles = _memoryCache.Get("roles") as List<Roles>;
            try
            {
                var applicationsModel = await GetApplications(filters);
                var managers = await _queueSender.GetAllManagers();
                applicationsModel.Managers = managers;

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
            var token = HttpContext.Request.Cookies["AccessToken"];
            var userId = _tokenHelper.GetUserIdFromToken(token);
            filters.myId = userId;

            var applications = await _queueSender.GetApplications(filters);
            var managers = await _queueSender.GetAllManagers();

            var viewModel = new ApplicationsViewModel
            {
                ApplicationsResponse = applications,
                Filters = filters,
                Managers = managers,
                myId = userId
            };

            return viewModel;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateStatus(GetApplicationStatus data)
        {
            var changeStatus = new ChangeApplicationStatusDTO
            {
                ApplcationStatus = data.status,
                ApplicationId = data.applicationId
            };

            await _queueSender.SendMessage(changeStatus, QueueConst.ChangeApplicationStatusQueue);

            return RedirectToAction("Applications");
        }

        [HttpPost]
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

        private async Task<FacultiesResponseDTO> GetAllFaculties()
        {
            var faculties = await _queueSender.GetAllFaculties();

            return faculties;
        }
        private async Task<ProgramResponseDTO> GetAllPrograms()
        {
            var programs = await _queueSender.GetAllPrograms();

            return programs;
        }

        [Authorize(Roles = "MainManager")]
        public async Task<IActionResult> SetManagerOnApplication(TakeApplication data)
        {
            await _queueSender.SendMessage(data, QueueConst.SetManagerQueue);

            return RedirectToAction("Applications");
        }
    }
}
