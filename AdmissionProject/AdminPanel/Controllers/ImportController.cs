using AdminPanel.BL.Serivces.Interface;
using Common.Const;
using Common.DTO.Dictionary;
using Common.Enum;
using Common.Helpers;
using Microsoft.AspNetCore.Mvc;

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
        public async Task Import(ImportTypes importType)
        {
            var token = HttpContext.Request.Cookies["AccessToken"];
            var userId = _tokenHelper.GetUserIdFromToken(token);

            var importData = new MakeImportDTO
            {
                ImportTypes = importType, 
                UserId = userId
            };

            await _queueSender.SendMessage(importData, QueueConst.MakeImportQueue);

            await Import();

        }


        [HttpGet]
        public async Task<IActionResult> Import()
        {
            var history = await _queueSender.GetImportHistory();

            return View(history);
        }
    }
}
