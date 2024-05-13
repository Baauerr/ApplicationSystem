/*using EasyNetQ;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using NotificationService.DTO;
using RabbitMQ.Client;
using System.Text;

namespace NotificationService.Controllers
{
    [Route("api/notification")]
    public class NotificationController : Controller
    {

        private IBus _bus;

        public NotificationController()
        {
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        [HttpPost("sendNotify")]
        public async Task<ActionResult> LoginAsync()
        {

            var message = new MailStructure
            {
                Body = "Приходит как-то улитка в бар",
                Recipient = "umpa@lumpa.com",
                Subject = "halo"
            };

            await _bus.PubSub.PublishAsync(message);


            Console.WriteLine("Сообщение отправлено в очередь");

            return Ok();
        }   
    }
}*/