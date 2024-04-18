using DictionaryService.Common.DTO;
using DictionaryService.Common.Enums;
using DictionaryService.Common.Interfaces;
using DictionaryService.DAL.Entities;
using DictionaryService.DAL.Enum;
using Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NotificationService;
using System.Security.Claims;
using UserService.Controllers.Policies.HITSBackEnd.Controllers.AttributeUsage;

namespace UserService.Controllers
{
    [Route("api/dictionary")]
    public class DictionaryController : Controller
    {

        private readonly IImportService _importService;
        private readonly IDictionaryInfoService _infoService;

        public DictionaryController(IImportService importService, IDictionaryInfoService dictionaryInfoService )
        {
            _importService = importService;
            _infoService = dictionaryInfoService;

        }

        [HttpPost("import")]
        [Authorize(Roles = "Manager")]
        [ProducesResponseType(200)]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> ImportDictionary([FromQuery] OperationType operationType)
        {
            var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
            var userId = userIdClaim.Value;
            await _importService.ImportDictionary(operationType, Guid.Parse(userId));
            return Ok();
        }

        [HttpGet("programs")]
        [Authorize]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [ProducesResponseType(typeof(ProgramResponseDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult<ProgramResponseDTO>> GetPrograms(
            [FromQuery] string? name,
            [FromQuery] string? code,
            [FromQuery] List<Language>? language,
            [FromQuery] List<string>? educationForm,
            [FromQuery] List<string>? educationLevel,
            [FromQuery] List<string>? faculty,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10
            )
        {
            var programs = await _infoService.GetPrograms(
                name, code, language, educationForm, educationLevel, faculty, page, pageSize
                );

            return Ok(programs);
        }
        [HttpGet("facuilties")]
        [Authorize]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [ProducesResponseType(typeof(FacultiesResponseDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult<FacultiesResponseDTO>> GetFaculties([FromQuery] string facultyName)
        {
            
            return Ok(await _infoService.GetFaculties(facultyName));
        }

        [HttpGet("documentTypes")]
        [Authorize]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [ProducesResponseType(typeof(DocumentTypeResponseDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult<DocumentTypeResponseDTO>> GetDocumentTypes([FromQuery] string documentTypeName)
        {

            return Ok(await _infoService.GetDocumentTypes(documentTypeName));
        }

        [HttpGet("educationLevel")]
        [Authorize]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [ProducesResponseType(typeof(EducationLevelResponseDTO), 200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult<EducationLevelResponseDTO>> GetEducationLevel([FromQuery] string educationLevelName)
        {
            return Ok(await _infoService.GetEducationLevel(educationLevelName));
        }

        [HttpGet("history")]
        [Authorize(Roles = "Manager")]
        [ServiceFilter(typeof(TokenBlacklistFilterAttribute))]
        [ProducesResponseType(typeof(List<ImportHistory>), 200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult<ImportHistory>> GetImportHistory()
        {
            return Ok(await _infoService.GetImportHistory());
        }
    }

}