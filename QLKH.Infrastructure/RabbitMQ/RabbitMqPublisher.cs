using System.Text;
using System.Text.Json;
using QLKH.Application.Interfaces.Messaging;
using RabbitMQ.Client;

namespace QLKH.Infrastructure.RabbitMQ
{
    public class RabbitMqPublisher : IMessagePublisher
    {
        public void Publish<T>(string queueName, T message)
        {
            var factory = new ConnectionFactory
            {
                HostName = "localhost",
                UserName = "guest",
                Password = "guest"
            };

            using var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
            using var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();

            channel.QueueDeclareAsync(
                queue: queueName,
                durable: false,
                exclusive: false,
                autoDelete: false,
                arguments: null
            ).GetAwaiter().GetResult();

            var json = JsonSerializer.Serialize(message);
            var body = Encoding.UTF8.GetBytes(json);

            channel.BasicPublishAsync(
                exchange: "",
                routingKey: queueName,
                body: body
            ).GetAwaiter().GetResult();
        }
    }
}