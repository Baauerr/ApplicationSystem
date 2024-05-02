using Microsoft.Extensions.Hosting;
using RabbitMQ.Client.Events;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Common.DTO.User;
using UserService.Common.Interfaces;

namespace UserService.BL.Helpers

{
    internal class QueueListener : BackgroundService
    {

        private readonly IAccountService _accountService;

        public QueueListener(IAccountService accountService)
        {
            _accountService = accountService;
        }

        protected async override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            var factory = new ConnectionFactory()
            {
                HostName = "localhost",
            };

            var connection = factory.CreateConnection();
            using var channel = connection.CreateModel();

            channel.QueueDeclare(queue: "give_roles", durable: false, exclusive: false, autoDelete: false, arguments: null);

            var consumer = new EventingBasicConsumer(channel);
            consumer.Received += (model, ea) =>
            {
                var body = ea.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);
                var newEmail = JsonConvert.DeserializeObject<SetRoleRequestDTO>(message);
                _accountService.GiveRole(newEmail);
            };

            channel.BasicConsume(queue: "give_roles",
                                 autoAck: true,
                                 consumer: consumer);

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(1000, stoppingToken);
            }
        }


    }
}
