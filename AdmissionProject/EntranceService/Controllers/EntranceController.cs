using Common.DTO.Entrance;
using Common.Enum;
using Common.DTO.Document;
using EasyNetQ;
using EntranceService.BL.Services;
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
    //    [Authorize(Roles = "Entrant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult<ApplicationsResponseDTO>> GetApplications(
            [FromQuery] string? entrantName,
            [FromQuery] Guid? programsGuid,
            [FromQuery] List<string>? faculties,
            [FromQuery] ApplicationStatus? status,
            [FromQuery] bool? hasManager,
            [FromQuery] bool? onlyMyManaging,
            [FromQuery] Guid? managerId,
            [FromQuery] SortingTypes? sortingTypes,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;

            var aplicationInfo = await _entranceService.GetApplications(
            entrantName,
            programsGuid,
            faculties,
            status,
            hasManager,
            onlyMyManaging,
            managerId,
            sortingTypes,
            page,
            pageSize,
            Guid.Parse(userId)
            );

            return Ok(aplicationInfo);
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

        [HttpPut("agagagaga")]
      //  [Authorize(Roles = "Entrant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> GetManagers([FromBody] EditApplicationDTO applicationEditInfo)
        {
          //  var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
          //  var userId = userIdClaim.Value;
            var managers = _entranceService.GetAllManagers();
            return Ok(managers);
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
            await _entranceService.DeleteProgram(deleteProgramDTO, Guid.Parse(userId), null);
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
            await _entranceService.ChangeProgramPriority(changeProgramPriorityDTO, Guid.Parse(userId), null);
            return Ok();
        }

        [HttpPost("program")]
        [Authorize]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> AddProgramToApplicartion([FromBody] AddProgramsToApplicationDTO addProgramDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            await _entranceService.AddProgramsToApplication(addProgramDTO.programsDTO, addProgramDTO.applicationId, Guid.Parse(userId));
            return Ok();
        }

    }

}