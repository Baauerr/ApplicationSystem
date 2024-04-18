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

        [HttpPost("sendNotify")]
        public async Task<ActionResult> LoginAsync()
        {

                var factory = new ConnectionFactory()
                {
                    HostName = "localhost",
                };

                var connection = factory.CreateConnection();

                using var channel = connection.CreateModel();

                var message = new MailStructure
                {
                    body = "Приходит как-то улитка в бар",
                    recipient = "umpa@lumpa.com",
                    subject = "halo"
                };

                var jsonString = JsonConvert.SerializeObject(message);

                var body = Encoding.UTF8.GetBytes(jsonString);

                channel.QueueDeclare(queue: "notification", durable: false, exclusive: false, autoDelete: false, arguments: null);

                channel.BasicPublish(exchange: "", routingKey: "notification", basicProperties: null, body: body);

                Console.WriteLine("Сообщение отправлено в очередь");
            
            return Ok();
        }   
    }
}