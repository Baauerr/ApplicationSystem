using DocumentService.Common.DTO;
using DocumentService.Common.Interface;
using Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DocumentService.Controllers
{
    [ApiController]
    [Route("api/documents")]
    public class DocumentsController : Controller
    {

        private readonly IDocumentFormService _documentFormService;

        public DocumentsController(IDocumentFormService documentFormService)
        {
            _documentFormService = documentFormService;
        }

        [HttpPost("passportForm")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> AddPassportForm([FromBody] PassportFormDTO passportDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;

            await _documentFormService.AddPassportInfo(passportDTO, Guid.Parse(userId));
            return Ok();
        }

        [HttpPut("passportForm")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> EditPassportForm([FromBody] PassportFormDTO passportDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;

            await _documentFormService.EditPassportInfo(passportDTO, Guid.Parse(userId));
            return Ok();
        }
        [HttpDelete("passportForm")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> DeletePassportForm()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            await _documentFormService.DeletePassportInfo(Guid.Parse(userId));
            return Ok();
        }

        [HttpPost("educationDocumentForm")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> AddEducationForm([FromBody] EducationDocumentFormDTO educationDocDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            await _documentFormService.AddEducationDocumentsInfo(educationDocDTO, Guid.Parse(userId));
            return Ok();
        }
        [HttpDelete("educationDocumentForm")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> DeleteEducationForm([FromBody] DeleteEducationFormDTO educationDocDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            await _documentFormService.DeleteEducationDocumentsInfo(educationDocDTO, Guid.Parse(userId));
            return Ok();
        }
        [HttpPut("educationDocumentForm")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> EditEducationForm([FromBody] EducationDocumentFormDTO educationDocDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            await _documentFormService.EditEducationDocumentsInfo(educationDocDTO, Guid.Parse(userId));
            return Ok();
        }
    }
}
