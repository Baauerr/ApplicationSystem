using AdminPanel.BL.Serivces.Interface;
using AdminPanel.Models;
using Common.Const;
using Common.DTO.Profile;
using Common.Helpers;
using Common.DTO.Document;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis;
using Pipelines.Sockets.Unofficial.Buffers;
using Common.DTO.Entrance;

namespace AdminPanel.Controllers
{
    [Authorize(Roles = "Manager")]
    public class EntrantController : Controller
    {

        private readonly IQueueSender _queueSender;
        private readonly ITokenHelper _tokenHelper;
        public EntrantController(IQueueSender queueSender, ITokenHelper token)
        {
            _queueSender = queueSender;
            _tokenHelper = token;
        }

        public async Task<IActionResult> Entrant(Guid OwnerId)
        {
            var profileInfo = await _queueSender.GetProfile(OwnerId);

            var passportInfo = await _queueSender.GetPassportForm(OwnerId);

            var educationDocumentInfo  = await _queueSender.GetEducationDocumentForm(OwnerId);

            var allEducationLevels = await _queueSender.GetAllEducationLevels();

            var programs = await _queueSender.GetApplicationPrograms(OwnerId);

            var educationLevelView = new EducationDocumentViewModel
            {
                EducationDocumentForm = educationDocumentInfo,
                EducationLevel = allEducationLevels,
            };

            var entrantPageView = new EntrantViewModel
            {
                EducationDocumentForm = educationLevelView,
                PassportForm = passportInfo,
                Profile = profileInfo,
                ApplicationPrograms = programs
            };

            ViewBag.OwnerId = OwnerId;

            return View(entrantPageView);
        }

        [HttpPost]
        public async Task<IActionResult> Entrant(ChangeProfileRequestDTO newProfileInfo)
        {
            Guid OwnerId = Guid.Empty;
            if (Request.Query.ContainsKey("OwnerId"))
            {
                OwnerId = Guid.Parse(Request.Query["OwnerId"]);
                ViewBag.OwnerId = OwnerId;
            }

            var editProfileDTO = new ChangeProfileRequestRPCDTO
                {
                    ProfileData = newProfileInfo,
                    UserId = OwnerId
                };
                try
                {
                    await _queueSender.SendMessage(editProfileDTO, QueueConst.ChangeProfileQueue);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
            
            var profileInfo = await _queueSender.GetProfile(OwnerId);

            return PartialView("_EntrantProfilePartial", profileInfo);
        }


        [HttpPost]
        public async Task<IActionResult> EntrantPassport(PassportFormDTO newPassportInfo)
        {
            Guid OwnerId = Guid.Empty;
            if (Request.Query.ContainsKey("OwnerId"))
            {
                OwnerId = Guid.Parse(Request.Query["OwnerId"]);
                ViewBag.OwnerId = OwnerId;
            }


            var editPassportDTO = new EditPassportFormDTORPC
            {
                PassportInfo = newPassportInfo,
                UserId = OwnerId
            };
            try
            {
                await _queueSender.SendMessage(editPassportDTO, QueueConst.UpdateEntrantPassportQueue);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            var passportInfo = await _queueSender.GetPassportForm(OwnerId);
            return PartialView("_PassportPartial", passportInfo);
        }

        [HttpPost]
        public async Task<IActionResult> EntrantEducationDocument(EducationDocumentFormDTO newEducationDocumentInfo)
        {

            Guid OwnerId = Guid.Empty;
            if (Request.Query.ContainsKey("OwnerId"))
            {
                OwnerId = Guid.Parse(Request.Query["OwnerId"]);
                ViewBag.OwnerId = OwnerId;
            }

            var editEducationDocumentForm = new EditEducationDocumentFormRPC
            {
                EducationDocumentInfo = newEducationDocumentInfo,
                UserId = OwnerId
            };
            try
            {
                await _queueSender.SendMessage(editEducationDocumentForm, QueueConst.UpdateEducationDocumentFormQueue);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            var documentInfo = await _queueSender.GetEducationDocumentForm(OwnerId);
            var educationLeves = await _queueSender.GetAllEducationLevels();

            var documentView = new EducationDocumentViewModel
            {
                EducationDocumentForm = documentInfo,
                EducationLevel = educationLeves
            };

            return PartialView("_EducationDocumentPartial", documentInfo);
        }

        [HttpPost]
        public async Task<IActionResult> EntrantPrograms(ChangeProgramPriorityDTO info)
        {
            Guid OwnerId = Guid.Empty;
            if (Request.Query.ContainsKey("OwnerId"))
            {
                OwnerId = Guid.Parse(Request.Query["OwnerId"]);
                ViewBag.OwnerId = OwnerId;
            }

            var editPriorityDTO = new ChangeProgramPriorityDTORPC
            {
                programInfo = info,
                UserId = OwnerId
            };
            try
            {
                await _queueSender.SendMessage(editPriorityDTO, QueueConst.ChangeProgramPriorityQueue);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            var programs = await _queueSender.GetApplicationPrograms(OwnerId);
            return PartialView("_ProgramsPartial", programs);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteProgram(DeleteProgramDTO info)
        {
            Guid OwnerId = Guid.Empty;
            if (Request.Query.ContainsKey("OwnerId"))
            {
                OwnerId = Guid.Parse(Request.Query["OwnerId"]);
                ViewBag.OwnerId = OwnerId;
            }

            var deleteProgramDTO = new DeleteProgramDTORPC
            {
                deleteData = info,
                UserId = OwnerId
            };
            try
            {
                await _queueSender.SendMessage(deleteProgramDTO, QueueConst.RemoveProgramFromApplicationQueue);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = ex.Message;
            }

            var programs = await _queueSender.GetApplicationPrograms(OwnerId);
            return PartialView("_ProgramsPartial", programs);
        }
    }
}
