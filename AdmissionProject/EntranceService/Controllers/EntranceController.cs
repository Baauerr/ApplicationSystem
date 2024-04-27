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
        private readonly IDocumentService _documentService;

        public EntranceController(IEntranceService entranceService, IDocumentService documentService)
        {
            _entranceService = entranceService;
            _documentService = documentService;
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
        [HttpPost("educationDocumentForm")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> AddEducationForm([FromBody] EducationDocumentDTO educationDocDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            await _documentService.AddEducationDocumentsInfo(educationDocDTO, Guid.Parse(userId));
            return Ok();
        }

        [HttpPost("passportForm")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> AddPassportForm([FromBody] PassportInfoDTO passportDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            await _documentService.AddPassportInfo(passportDTO, Guid.Parse(userId));
            return Ok();
        }
    }

}