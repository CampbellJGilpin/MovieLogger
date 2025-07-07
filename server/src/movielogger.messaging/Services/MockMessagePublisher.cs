using movielogger.messaging.Services;
using movielogger.messaging.Models;

namespace movielogger.messaging.Services
{
    public class MockMessagePublisher : IMessagePublisher
    {
        public Task PublishAsync<T>(T message) where T : MovieEvent
        {
            return Task.CompletedTask;
        }

        public Task PublishAsync<T>(T message, string routingKey) where T : MovieEvent
        {
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            // Nothing to dispose
        }
    }
} 