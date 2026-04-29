using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

using ConsumerEnrollmentCreatedMessage = QLKH.Consumer.EnrollmentCreatedMessage;
using AdminNotificationMessage = QLKH.Application.Messages.AdminNotificationMessage;

Console.OutputEncoding = Encoding.UTF8;

var factory = new ConnectionFactory
{
    HostName = "localhost",
    UserName = "guest",
    Password = "guest"
};

var connection = factory.CreateConnectionAsync().GetAwaiter().GetResult();
var channel = connection.CreateChannelAsync().GetAwaiter().GetResult();

const string enrollmentQueueName = "enrollment-created-queue";
const string adminNotificationQueueName = "admin-notification-queue";

channel.QueueDeclareAsync(
    queue: enrollmentQueueName,
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null
).GetAwaiter().GetResult();

channel.QueueDeclareAsync(
    queue: adminNotificationQueueName,
    durable: false,
    exclusive: false,
    autoDelete: false,
    arguments: null
).GetAwaiter().GetResult();

Console.WriteLine("==========================================");
Console.WriteLine("RabbitMQ Consumer is running...");
Console.WriteLine($"Listening queue: {enrollmentQueueName}");
Console.WriteLine($"Listening queue: {adminNotificationQueueName}");
Console.WriteLine("==========================================");

var enrollmentConsumer = new AsyncEventingBasicConsumer(channel);

enrollmentConsumer.ReceivedAsync += async (model, ea) =>
{
    try
    {
        var body = ea.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);

        var message = JsonSerializer.Deserialize<ConsumerEnrollmentCreatedMessage>(json);

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
        Console.WriteLine("Enrollment consumer error: " + ex.Message);

        await channel.BasicNackAsync(
            deliveryTag: ea.DeliveryTag,
            multiple: false,
            requeue: false);
    }
};

var adminNotificationConsumer = new AsyncEventingBasicConsumer(channel);

adminNotificationConsumer.ReceivedAsync += async (model, ea) =>
{
    try
    {
        var body = ea.Body.ToArray();
        var json = Encoding.UTF8.GetString(body);

        var message = JsonSerializer.Deserialize<AdminNotificationMessage>(json);

        Console.WriteLine();
        Console.WriteLine("=== Received Admin Notification Message ===");
        Console.WriteLine($"Subject    : {message?.Subject}");
        Console.WriteLine($"Message    : {message?.Message}");
        Console.WriteLine($"Target     : {message?.TargetText}");
        Console.WriteLine($"Sent       : {message?.SentCount}");
        Console.WriteLine($"Failed     : {message?.FailedCount}");
        Console.WriteLine($"Attachment : {message?.AttachmentFileName}");
        Console.WriteLine($"CreatedBy  : {message?.CreatedBy}");
        Console.WriteLine($"CreatedAt  : {message?.CreatedAt}");
        Console.WriteLine("===========================================");
        Console.WriteLine();

        await channel.BasicAckAsync(ea.DeliveryTag, multiple: false);
    }
    catch (Exception ex)
    {
        Console.WriteLine("Admin notification consumer error: " + ex.Message);

        await channel.BasicNackAsync(
            deliveryTag: ea.DeliveryTag,
            multiple: false,
            requeue: false);
    }
};

await channel.BasicConsumeAsync(
    queue: enrollmentQueueName,
    autoAck: false,
    consumer: enrollmentConsumer);

await channel.BasicConsumeAsync(
    queue: adminNotificationQueueName,
    autoAck: false,
    consumer: adminNotificationConsumer);

Console.WriteLine("Press [Enter] to exit.");
Console.ReadLine();

await channel.CloseAsync();
await connection.CloseAsync();