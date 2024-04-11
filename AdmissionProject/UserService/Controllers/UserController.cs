using Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using UserService.Common.DTO.Auth;
using UserService.Common.DTO.Profile;
using UserService.Common.Interfaces;
using UserService.Controllers.Policies.HITSBackEnd.Controllers.AttributeUsage;

namespace UserService.Controllers
{
    [Route("api/user")]
    public class UserController : Controller
    {

        private readonly IAccountService _userService;
        private readonly IAuthService _authService;
        private readonly ITokenService _tokenService;

        public UserController(IAccountService service, IAuthService authService, ITokenService tokenService)
        {
            _userService = service;
            _authService = authService;
            _tokenService = tokenService;
        }

        [HttpPost("login")]
        [ProducesResponseType(typeof(AuthResponseDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult<AuthResponseDTO>> LoginAsync([FromBody] LoginRequestDTO loginProps)
        {
            return Ok(await _authService.Login(loginProps));
        }
        [HttpPost("register")]
        [ProducesResponseType(typeof(AuthResponseDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        public async Task<ActionResult<AuthResponseDTO>> RegisterAsync([FromBody] RegistrationRequestDTO registrationProps)
        {
            return Ok(await _authService.Register(registrationProps));
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [HttpGet("profile")]
        [ProducesResponseType(typeof(ProfileResponseDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 401)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 404)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult<ProfileResponseDTO>> GetProfileAsync()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            return Ok(await _userService.GetProfile(token));
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPut("profile")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 401)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 404)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> ChangeProfileAsync([FromBody] ChangeProfileRequestDTO newProfileInfo)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _userService.ChangeProfileInfo(newProfileInfo, token);
            return Ok();
        }

        [HttpPost("refresh")]
        [ProducesResponseType(typeof(RefreshedToken), 200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 404)]
        public async Task<ActionResult<AuthResponseDTO>> RefreshAsync([FromBody] RefreshTokenRequestDTO refreshToken)
        {
            return Ok(await _tokenService.Refresh(refreshToken.token));
        }


        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpPost("logout")]
        public async Task<ActionResult<AuthResponseDTO>> Logout()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            await _authService.Logout(token);
            return Ok();
        }

        [Authorize(AuthenticationSchemes = "Bearer")]
        [HttpGet("getMyRoles")]
        [ProducesResponseType(typeof(UserRoleResponseDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 404)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult<AuthResponseDTO>> GetMyRoles()
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            return Ok(await _userService.GetMyRoles(token));
        }
    }

}