using movielogger.messaging.Services;
using movielogger.messaging.Models;

namespace movielogger.messaging.Services
{
    public class MockMessageConsumer : IMessageConsumer
    {
        public event EventHandler<MovieEvent> MessageReceived = null!;

        public void StartConsuming()
        {
            // Do nothing for testing
        }

        public void StopConsuming()
        {
            // Do nothing for testing
        }
    }
}