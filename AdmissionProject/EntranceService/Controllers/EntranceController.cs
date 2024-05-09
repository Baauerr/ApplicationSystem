using DocumentService.Common.DTO;
using EasyNetQ;
using EntranceService.BL.Services;
using EntranceService.Common.DTO;
using EntranceService.Common.Interface;
using Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;


namespace EntranceService.Controllers
{
    [Route("api/entrance")]
    public class EntranceController : Controller
    {

        private readonly IEntranceService _entranceService;
        private readonly IEntrantService _entrantService;
        private readonly QueueSender _queueSender;

        public EntranceController(IEntranceService entranceService, IEntrantService entrantService, QueueSender queueSender)
        {
            _entranceService = entranceService;
            _entrantService = entrantService;
            _queueSender = queueSender;
        }

        [HttpGet("application")]
        [Authorize(Roles = "Entrant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> ChangeProgramPriority()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            var aaplicationInfo = await _entrantService.GetApplicationInfo(Guid.Parse(userId));
            return Ok(aaplicationInfo);
        }

        [HttpPost("application")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> CreateApplication([FromBody] CreateApplicationDTO applicationDTO)
        {
            string token = HttpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            await _entranceService.CreateApplication(Guid.Parse(userId), token, applicationDTO);
            return Ok();
        }

        [HttpPut("application")]
        [Authorize(Roles = "Entrant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> EditApplication([FromBody] EditApplicationDTO applicationEditInfo)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            await _entrantService.EditApplicationsInfo(applicationEditInfo, Guid.Parse(userId));
            return Ok();
        }
        [HttpDelete("program")]
        [Authorize(Roles = "Entrant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> DeleteProgram([FromBody] DeleteProgramDTO deleteProgramDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            await _entranceService.DeleteProgram(deleteProgramDTO, Guid.Parse(userId));
            return Ok();
        }
        
        [HttpPut("program")]
        [Authorize(Roles = "Entrant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> ChangeProgramPriority([FromBody] ChangeProgramPriorityDTO changeProgramPriorityDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            await _entranceService.ChangeProgramPriority(changeProgramPriorityDTO, Guid.Parse(userId));
            return Ok();
        }

        [HttpPut("AGADA")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> passp([FromBody] ChangeProgramPriorityDTO changeProgramPriorityDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var _bus = RabbitHutch.CreateBus("host=localhost");
            var userId = userIdClaim.Value;
            var passport = await _bus.Rpc.RequestAsync<UserIdDTO, GetPassportFormDTO>(new UserIdDTO { UserId = Guid.Parse(userId) });
            return Ok(passport);
        }
    }

}