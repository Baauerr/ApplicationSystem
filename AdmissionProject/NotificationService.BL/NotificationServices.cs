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
    public class NotificationServices
    {

        private IBus _bus;


        public NotificationServices()
        {
            _bus = RabbitHutch.CreateBus("host=localhost");
        }

        public async Task StartListening()
        {
            await _bus.PubSub.SubscribeAsync<MailStructure>("notification", SendNotificationAsync);
        }

        public void SendNotificationAsync(MailStructure mailStructure)
        {
            SmtpClient client = new SmtpClient("localhost", 1025);

            MailMessage message = new MailMessage();
            message.From = new MailAddress("gamarjoba@allow.com");

            if (!IsValidEmail(mailStructure.Recipient))
            {
                throw new BadRequestException("Неверный формат email");
            }

            message.To.Add(new MailAddress(mailStructure.Recipient));
            message.Subject = mailStructure.Subject;
            message.Body = mailStructure.Body;

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
