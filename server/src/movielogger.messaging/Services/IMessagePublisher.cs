using movielogger.messaging.Models;

namespace movielogger.messaging.Services
{
    public interface IMessagePublisher
    {
        Task PublishAsync<T>(T message) where T : MovieEvent;
        Task PublishAsync<T>(T message, string routingKey) where T : MovieEvent;
        void Dispose();
    }
} 