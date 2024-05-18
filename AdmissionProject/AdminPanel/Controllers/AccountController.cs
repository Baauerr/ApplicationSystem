using AdminPanel.BL.Serivces.Interface;
using Common.Const;
using Common.DTO.Auth;
using Common.DTO.Profile;
using Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NuGet.Common;
using NuGet.Protocol.Plugins;

namespace AdminPanel.Controllers
{
    public class AccountController : Controller
    {

        private readonly IQueueSender _queueSender;
        private readonly ITokenHelper _tokenHelper;
        public AccountController(IQueueSender queueSender, ITokenHelper token) {
            _queueSender = queueSender;
            _tokenHelper = token;
        }

        [HttpGet]
        public IActionResult Login()
        {
            return View();
        }

        [HttpGet]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> Profile()
        {
            var token = HttpContext.Session.GetString("AccessToken");

            var userId = _tokenHelper.GetUserIdFromToken(token);

            var profileInfo = await _queueSender.GetProfile(userId);

            return View(profileInfo);
        }

        [HttpPost]
        public async Task<IActionResult> Login(LoginRequestDTO model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var loginCreds = new LoginRequestDTO
                    {
                        Email = model.Email,
                        Password = model.Password
                    };

                    var loginResponse = await _queueSender.Login(loginCreds);

                    HttpContext.Session.SetString("Email", loginCreds.Email);
                    HttpContext.Session.SetString("AccessToken", loginResponse.AccessToken);
                    HttpContext.Session.SetString("RefreshToken", loginResponse.RefreshToken);

                    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }
            }

            return View();
        }

        [HttpPost]
        public async Task<IActionResult> ChangePassword(PasswordChangeRequestDTO newPasswordData)
        {

            var token = HttpContext.Session.GetString("AccessToken");

            if (token != null)
            {
                var passwordChangeRequest = new PasswordChangeRequestRPCDTO
                {
                    passwordInfo = newPasswordData,
                    UserId = _tokenHelper.GetUserIdFromToken(token)
                };



                try
                {
                    await _queueSender.SendMessage(passwordChangeRequest, QueueConst.ChangePasswordQueue);
                    
                //    return RedirectToAction("Index", "Home");
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }

            }

            return View();
        }


            [HttpPost]
            public async Task<IActionResult> Profile(ChangeProfileRequestDTO newProfileInfo)
            {

                var token = HttpContext.Session.GetString("AccessToken");

            var userId = _tokenHelper.GetUserIdFromToken(token); 

                if (token != null)
                {
                    var editProfileDTO = new ChangeProfileRequestRPCDTO
                    {
                        ProfileData = newProfileInfo,
                        UserId = userId
                    };
                try
                {
                    await _queueSender.SendMessage(editProfileDTO, QueueConst.ChangeProfileQueue);
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = ex.Message;
                }              
            }

            var profileInfo = await _queueSender.GetProfile(userId);

            return View(profileInfo);
            }
        }
}

