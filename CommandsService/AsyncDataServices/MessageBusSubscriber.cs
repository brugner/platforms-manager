using System.Text;
using CommandsService.EventProcessing;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace CommandsService.AsyncDataServices;

public class MessageBusSubscriber : BackgroundService
{
    private readonly IConfiguration _configuration;
    private readonly IEventProcessor _eventProcessor;
    private IConnection _connection = default!;
    private IModel _channel = default!;
    private string _queueName = default!;

    public MessageBusSubscriber(IConfiguration configuration, IEventProcessor eventProcessor)
    {
        _configuration = configuration;
        _eventProcessor = eventProcessor;

        InitializeRabbitMQ();
    }

    protected override Task ExecuteAsync(CancellationToken stoppingToken)
    {
        stoppingToken.ThrowIfCancellationRequested();

        var consumer = new EventingBasicConsumer(_channel);

        consumer.Received += OnRabbitMQ_Received;

        _channel.BasicConsume(queue: _queueName, autoAck: true, consumer: consumer);

        return Task.CompletedTask;
    }

    private void OnRabbitMQ_Received(object? sender, BasicDeliverEventArgs e)
    {
        System.Console.WriteLine("--> Event received!");

        var body = e.Body;
        var notificationMessage = Encoding.UTF8.GetString(body.ToArray());

        _eventProcessor.ProcessEvent(notificationMessage);
    }

    private void InitializeRabbitMQ()
    {
        var factory = new ConnectionFactory { HostName = _configuration["RabbitMQHost"], Port = int.Parse(_configuration["RabbitMQPort"]) };
        _connection = factory.CreateConnection();
        _channel = _connection.CreateModel();
        _channel.ExchangeDeclare(exchange: "trigger", type: ExchangeType.Fanout);
        _queueName = _channel.QueueDeclare().QueueName;
        _channel.QueueBind(queue: _queueName, exchange: "trigger", routingKey: string.Empty);

        System.Console.WriteLine("--> Listening on the message bus..");

        _connection.ConnectionShutdown += OnRabbitMQ_ConnectionShutdown;
    }

    private void OnRabbitMQ_ConnectionShutdown(object? sender, ShutdownEventArgs e)
    {
        System.Console.WriteLine("--> Connection shutdown");
    }

    public override void Dispose()
    {
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }

        base.Dispose();
    }
}