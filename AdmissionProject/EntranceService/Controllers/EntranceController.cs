using Common.DTO.Entrance;
using Common.Enum;
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

        public EntranceController(IEntranceService entranceService, IEntrantService entrantService)
        {
            _entranceService = entranceService;
            _entrantService = entrantService;
        }

        [HttpGet("application")]
        [Authorize(Roles = "Manager")]
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

        [HttpGet("application/my")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(typeof(GetApplicationDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 404)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> GetMyApplication()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            var application = await _entrantService.GetApplicationInfo(Guid.Parse(userId));
            return Ok(application);
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
        [Authorize(Roles = "Entrant")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> AddProgramToApplication([FromBody] AddProgramsToApplicationDTO addProgramDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            await _entranceService.AddProgramsToApplication(addProgramDTO.programsDTO, addProgramDTO.applicationId, Guid.Parse(userId));
            return Ok();
        }
        [HttpGet("program/my")]
        [Authorize(Roles = "Entrant")]
        [ProducesResponseType(typeof(GetApplicationPrograms),200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> GetMyPrograms()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            var programs = await _entranceService.GetApplicationPrograms(Guid.Parse(userId));
            return Ok(programs);
        }

    }

}