using AdminPanel.BL.Serivces.Interface;
using AdminPanel.Models;
using Common.Const;
using Common.DTO.Profile;
using Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

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

            var entrantPageView = new EntrantViewModel
            {
                EducationDocumentForm = educationDocumentInfo,
                PassportForm = passportInfo,
                Profile = profileInfo,
            };

            return View(entrantPageView);
        }

        [HttpPost]
        public async Task<IActionResult> Entrant(ChangeProfileRequestDTO newProfileInfo, Guid OwnerId)
        {
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

            return View(profileInfo);
        }
    }
}
