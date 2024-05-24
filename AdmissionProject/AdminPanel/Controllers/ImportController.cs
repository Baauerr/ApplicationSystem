using AdminPanel.BL.Serivces.Interface;
using Common.Const;
using Common.DTO.Dictionary;
using Common.Enum;
using Common.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace AdminPanel.Controllers
{
    public class ImportController : Controller
    {

        private readonly IQueueSender _queueSender;
        private readonly ITokenHelper _tokenHelper;
        public ImportController(IQueueSender queueSender, ITokenHelper token)
        {
            _queueSender = queueSender;
            _tokenHelper = token;
        }
        [HttpPost]
        public async Task<IActionResult> Import(ImportTypes importType)
        {
            var token = HttpContext.Request.Cookies["AccessToken"];
            var userId = _tokenHelper.GetUserIdFromToken(token);

            var importData = new MakeImportDTO
            {
                ImportTypes = importType,
                UserId = userId
            };

            await _queueSender.SendMessage(importData, QueueConst.MakeImportQueue);

            var history = await _queueSender.GetImportHistory();
            return PartialView("_ImportHistory", history);
        }

        [HttpGet]
        public async Task<IActionResult> Import()
        {
            var history = await _queueSender.GetImportHistory();

            return View(history);
        }
    }
}
