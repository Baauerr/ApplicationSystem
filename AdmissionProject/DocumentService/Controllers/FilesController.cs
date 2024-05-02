using DocumentService.Common.DTO;
using DocumentService.Common.Interface;
using Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace DocumentService.Controllers
{
    [ApiController]
    [Route("api/files")]
    public class FilesController : Controller
    {

        private readonly IFileService _fileService;

        public FilesController(IFileService fileService)
        {
            _fileService = fileService;
        }

        [HttpPost]
        [Route("passport")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> UpLoadPassport(IFormFile file)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;

            await _fileService.UploadPassportFile(file, Guid.Parse(userId));
            return Ok();
        }
        [HttpPost]
        [Route("educationDocument")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> UpLoadEducationDocument(IFormFile file, EducationFileDTO educationFileDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;

            await _fileService.UploadEducationDocumentFile(file, Guid.Parse(userId), educationFileDTO);
            return Ok();
        }

        [HttpDelete]
        [Route("passport")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> DeletePassport()
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;

            await _fileService.DeletePassportFile(Guid.Parse(userId));
            return Ok();
        }
        [HttpDelete]
        [Route("educationDocument")]
        [Authorize(Roles = "User")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(Error), 400)]
        [ProducesResponseType(typeof(Error), 401)]
        [ProducesResponseType(typeof(Error), 500)]
        public async Task<ActionResult> DeleteEducationDocument(DeleteEducationFormDTO educationFileDTO)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;

            await _fileService.DeleteEducationDocumentFile(Guid.Parse(userId), educationFileDTO);
            return Ok();
        }

    }
}
