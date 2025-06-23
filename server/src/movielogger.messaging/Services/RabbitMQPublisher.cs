using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using movielogger.messaging.Configuration;
using movielogger.messaging.Models;
using RabbitMQ.Client;

namespace movielogger.messaging.Services
{
    public class RabbitMQPublisher : IMessagePublisher, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQPublisher> _logger;
        private readonly RabbitMQConfig _config;
        private bool _disposed;

        public RabbitMQPublisher(IOptions<RabbitMQConfig> config, ILogger<RabbitMQPublisher> logger)
        {
            _config = config.Value;
            _logger = logger;

            var factory = new ConnectionFactory
            {
                HostName = _config.Host,
                Port = _config.Port,
                UserName = _config.Username,
                Password = _config.Password,
                VirtualHost = _config.VirtualHost
            };

            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();

            // Declare exchange
            _channel.ExchangeDeclare(
                exchange: _config.ExchangeName,
                type: ExchangeType.Topic,
                durable: true,
                autoDelete: false);

            // Declare queue
            _channel.QueueDeclare(
                queue: _config.QueueName,
                durable: true,
                exclusive: false,
                autoDelete: false);

            // Bind queue to exchange with wildcard routing
            _channel.QueueBind(
                queue: _config.QueueName,
                exchange: _config.ExchangeName,
                routingKey: "#"); // # matches zero or more words

            _logger.LogInformation("RabbitMQ publisher initialized successfully");
        }

        public async Task PublishAsync<T>(T message) where T : MovieEvent
        {
            await PublishAsync(message, message.RoutingKey);
        }

        public async Task PublishAsync<T>(T message, string routingKey) where T : MovieEvent
        {
            try
            {
                var json = JsonSerializer.Serialize(message, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                var body = Encoding.UTF8.GetBytes(json);

                var properties = _channel.CreateBasicProperties();
                properties.Persistent = true;
                properties.ContentType = "application/json";
                properties.MessageId = Guid.NewGuid().ToString();
                properties.Timestamp = new AmqpTimestamp(DateTimeOffset.UtcNow.ToUnixTimeSeconds());

                _channel.BasicPublish(
                    exchange: _config.ExchangeName,
                    routingKey: routingKey,
                    basicProperties: properties,
                    body: body);

                _logger.LogInformation("Message published successfully. Type: {EventType}, RoutingKey: {RoutingKey}", 
                    message.EventType, routingKey);

                await Task.CompletedTask;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error publishing message. Type: {EventType}, RoutingKey: {RoutingKey}", 
                    message.EventType, routingKey);
                throw;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed && disposing)
            {
                _channel?.Close();
                _connection?.Close();
                _channel?.Dispose();
                _connection?.Dispose();
                _disposed = true;
            }
        }
    }
} 