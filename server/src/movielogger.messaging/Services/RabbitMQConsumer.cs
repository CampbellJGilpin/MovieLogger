using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using movielogger.messaging.Configuration;
using movielogger.messaging.Models;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace movielogger.messaging.Services
{
    public class RabbitMQConsumer : IMessageConsumer, IDisposable
    {
        private readonly IConnection _connection;
        private readonly IModel _channel;
        private readonly ILogger<RabbitMQConsumer> _logger;
        private readonly RabbitMQConfig _config;
        private readonly EventingBasicConsumer _consumer;
        private bool _disposed;
        private bool _isConsuming;

        public event EventHandler<MovieEvent>? MessageReceived;

        public RabbitMQConsumer(IOptions<RabbitMQConfig> config, ILogger<RabbitMQConsumer> logger)
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
                routingKey: "#");

            // Set up consumer
            _consumer = new EventingBasicConsumer(_channel);
            _consumer.Received += OnMessageReceived;

            _logger.LogInformation("RabbitMQ consumer initialized successfully");
        }

        public void StartConsuming()
        {
            if (_isConsuming)
            {
                _logger.LogWarning("Consumer is already running");
                return;
            }

            try
            {
                _channel.BasicConsume(
                    queue: _config.QueueName,
                    autoAck: false,
                    consumer: _consumer);

                _isConsuming = true;
                _logger.LogInformation("Started consuming messages from queue: {QueueName}", _config.QueueName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error starting consumer");
                throw;
            }
        }

        public void StopConsuming()
        {
            if (!_isConsuming)
            {
                _logger.LogWarning("Consumer is not running");
                return;
            }

            try
            {
                _channel.BasicCancel(_consumer.ConsumerTags.First());
                _isConsuming = false;
                _logger.LogInformation("Stopped consuming messages");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error stopping consumer");
                throw;
            }
        }

        private void OnMessageReceived(object? sender, BasicDeliverEventArgs e)
        {
            try
            {
                var body = e.Body.ToArray();
                var message = Encoding.UTF8.GetString(body);

                _logger.LogInformation("Received message: {Message}", message);

                // Try to deserialize as MovieEvent
                var movieEvent = JsonSerializer.Deserialize<MovieEvent>(message, new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

                if (movieEvent != null)
                {
                    MessageReceived?.Invoke(this, movieEvent);
                    _channel.BasicAck(e.DeliveryTag, false);
                    _logger.LogInformation("Message processed successfully. Type: {EventType}", movieEvent.EventType);
                }
                else
                {
                    _logger.LogWarning("Failed to deserialize message as MovieEvent");
                    _channel.BasicNack(e.DeliveryTag, false, true);
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing message");
                _channel.BasicNack(e.DeliveryTag, false, true);
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
                if (_isConsuming)
                {
                    StopConsuming();
                }

                _channel?.Close();
                _connection?.Close();
                _channel?.Dispose();
                _connection?.Dispose();
                _disposed = true;
            }
        }
    }
} 