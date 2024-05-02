using Newtonsoft.Json;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EntranceService.BL.Services
{
    public class QueueManager : IDisposable
    {
        private readonly ConnectionFactory _factory;
        private readonly IConnection _connection;

        public QueueManager()
        {
            _factory = new ConnectionFactory() { HostName = "localhost" };
            _connection = _factory.CreateConnection();
        }

        public void SendMessage<T>(string queueName, T message)
        {
            using (var channel = _connection.CreateModel())
            {
                var jsonString = JsonConvert.SerializeObject(message);
                var body = Encoding.UTF8.GetBytes(jsonString);

                channel.QueueDeclare(queue: queueName, durable: false, exclusive: false, autoDelete: false, arguments: null);
                channel.BasicPublish(exchange: "", routingKey: queueName, basicProperties: null, body: body);

                Console.WriteLine("Сообщение отправлено в очередь");
            }
        }

        public void Dispose()
        {
            _connection.Dispose();
        }
    }
}
