using System.Text;
using System.Text.Json;
using PlatformService.DTOs;
using RabbitMQ.Client;

namespace PlatformService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection _connection = default!;
    private readonly IModel _channel = default!;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;

        var factory = new ConnectionFactory { HostName = _configuration["RabbitMQHost"], Port = int.Parse(_configuration["RabbitMQPort"]!) };

        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
            _connection.ConnectionShutdown += OnRabbitMQ_ConnectionShutdown;

            System.Console.WriteLine("--> Connected to message bus");
        }
        catch (Exception ex)
        {
            System.Console.WriteLine($"--> Could not connect to the message bus: {ex.Message}");
        }
    }

    public void PublishNewPlatform(PlatformPublishedDTO platform)
    {
        var message = JsonSerializer.Serialize(platform);

        if (_connection.IsOpen)
        {
            System.Console.WriteLine("--> RabbitMQ connection is open, sending message..");
            SendMessage(message);
        }
        else
        {
            System.Console.WriteLine("--> RabbitMQ connection is closed, not sending message..");
        }
    }

    public void Dispose()
    {
        System.Console.WriteLine("MessageBus disposed");

        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }

    private void OnRabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        System.Console.WriteLine("--> RabbitMQ Connection Shutdown");
    }

    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(exchange: "trigger", routingKey: string.Empty, basicProperties: null, body: body);
        System.Console.WriteLine($"--> Message sent to RabbitMQ: {message}");
    }
}