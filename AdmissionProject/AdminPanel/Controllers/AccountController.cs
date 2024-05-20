using AdminPanel.BL.Serivces.Interface;
using Common.Const;
using Common.DTO.Auth;
using Common.DTO.Profile;
using Common.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;


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
        [Authorize]
        public IActionResult ChangePassword()
        {
            return View();
        }

        [HttpGet]
       // [Authorize]
        public async Task<IActionResult> Profile()
        {
            var token = HttpContext.Request.Cookies["AccessToken"];

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
                    Response.Cookies.Append("AccessToken", loginResponse.AccessToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddHours(1)
                    });

                    Response.Cookies.Append("RefreshToken", loginResponse.RefreshToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddDays(10)
                    });

                    return Json(new { success = true, redirectUrl = Url.Action("Index", "Home") });
                }
                catch (Exception ex)
                {
                    return Json(new { success = false, message = ex.Message });
                }
            }

            return Json(new { success = false, message = "Неправильный логин или пароль" });
        }

    [HttpPost]
      //  [Authorize]
        public async Task<IActionResult> ChangePassword(PasswordChangeRequestDTO newPasswordData)
        {

            var token = HttpContext.Request.Cookies["AccessToken"];

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
            var token = HttpContext.Request.Cookies["AccessToken"];

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

            return Json(profileInfo);
        }



        //  [HttpPost]
        //   [Authorize]
        public async Task<IActionResult> Logout()
        {

            Response.Cookies.Delete("AccessToken", new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1) 
            });

            Response.Cookies.Delete("RefreshToken", new CookieOptions
            {
                Expires = DateTime.Now.AddDays(-1)
            });

            return RedirectToAction("Index", "Home");
        }
    }
}


