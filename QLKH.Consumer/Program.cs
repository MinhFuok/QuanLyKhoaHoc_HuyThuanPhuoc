using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using QLKH.Consumer;

var factory = new ConnectionFactory
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest"
};

var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();

channel.QueueDeclareAsync(
    queue: "enrollment-created-queue",
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null
).GetAwaiter().GetResult();

Console.WriteLine("==========================================");
Console.WriteLine("RabbitMQ Consumer is running...");
Console.WriteLine("Listening queue: enrollment-created-queue");
Console.WriteLine("==========================================");

var consumer = new AsyncEventingBasicConsumer(channel);

consumer.ReceivedAsync += async (model, ea) =>
{
    try
    {
        var body = ea.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);

        var message = JsonSerializer.Deserialize<EnrollmentCreatedMessage>(json);

        Console.WriteLine();
        Console.WriteLine("=== Received Enrollment Message ===");
        Console.WriteLine($"EnrollmentId : {message?.EnrollmentId}");
        Console.WriteLine($"StudentId    : {message?.StudentId}");
        Console.WriteLine($"ClassRoomId  : {message?.ClassRoomId}");
        Console.WriteLine($"EnrolledAt   : {message?.EnrolledAt}");
        Console.WriteLine($"Status       : {message?.Status}");
        Console.WriteLine("===================================");
        Console.WriteLine();

        await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Consumer error: " + ex.Message);
    }
};

await channel.BasicConsumeAsync(
    queue: "enrollment-created-queue",
    autoAck: false,
    consumer: consumer);

Console.WriteLine("Press [Enter] to exit.");
Console.ReadLine();

await channel.CloseAsync();
await connection.CloseAsync();