using DictionaryService.Common.DTO;
using DictionaryService.Common.Enums;
using DictionaryService.Common.Interfaces;
using DictionaryService.DAL.Enum;
using Exceptions;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;
using System.ComponentModel.Design;

namespace UserService.Controllers
{
    [Route("api/dictionary")]
    public class DictionaryController : Controller
    {

        private readonly IImportService _importService;
        private readonly IDictionaryInfoService _infoService;

        public DictionaryController(IImportService importService, IDictionaryInfoService dictionaryInfoService)
        {
            _importService = importService;
            _infoService = dictionaryInfoService;
        }

        [HttpPost("import")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 400)]
        [ProducesResponseType(typeof(ExceptionResponseModel), 500)]
        public async Task<ActionResult> ImportDictionary()
        {
            await _importService.ImportDictionary(OperationType.Program);
            return Ok();
        }

        [HttpGet("programs")]
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

            foreach( var program in programs.programs )
            {
                Console.WriteLine(program.Name);
            }

            return Ok(programs);
        }


    }

}