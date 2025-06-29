namespace movielogger.messaging.Configuration
{
    public class RabbitMQConfig
    {
        public string Host { get; set; } = "localhost";
        public int Port { get; set; } = 5672;
        public string Username { get; set; } = "guest";
        public string Password { get; set; } = "guest";
        public string VirtualHost { get; set; } = "/";
        public string ExchangeName { get; set; } = "movielogger.events";
        public string QueueName { get; set; } = "movielogger.notifications";
    }
} 