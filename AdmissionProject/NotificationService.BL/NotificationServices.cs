using EasyNetQ;
using EasyNetQ.Consumer;
using Exceptions.ExceptionTypes;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using NotificationService.DTO;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System.Net.Mail;
using System.Text;
using System.Threading.Channels;

namespace NotificationService
{
    public class NotificationServices: BackgroundService
    {

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
            };

            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "notification", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var newEmail = JsonConvert.DeserializeObject<MailStructure>(message);
                SendNotification(newEmail);
            };

            channel.BasicConsume(queue: "notification",
                                 autoAck: true,
                                 consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }

        public void SendNotification(MailStructure mailStructure)
        {
            SmtpClient client = new SmtpClient("localhost", 1025);

            MailMessage message = new MailMessage();
            message.From = new MailAddress("gamarjoba@allow.com");

            if (!IsValidEmail(mailStructure.recipient))
            {
                throw new BadRequestException("Неверный формат email");
            }

            message.To.Add(new MailAddress(mailStructure.recipient));
            message.Subject = mailStructure.subject;
            message.Body = mailStructure.body;

            try
            {
                client.EnableSsl = false;
                client.Send(message);
                Console.WriteLine("Оповещение успешно отправлено");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Ошибка при отправке оповещение: " + ex.Message);
            }
        }
        private bool IsValidEmail(string emailAddress)
        {
            try
            {
                var address = new System.Net.Mail.MailAddress(emailAddress);
                return address.Address == emailAddress;
            }
            catch
            {
                return false;
            }
        }
    }
}
