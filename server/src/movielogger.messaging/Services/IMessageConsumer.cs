using movielogger.messaging.Models;

namespace movielogger.messaging.Services
{
    public interface IMessageConsumer
    {
        void StartConsuming();
        void StopConsuming();
        event EventHandler<MovieEvent> MessageReceived;
    }
}