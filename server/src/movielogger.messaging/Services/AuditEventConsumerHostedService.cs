using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.DependencyInjection;
using System.Threading;
using System.Threading.Tasks;

namespace movielogger.messaging.Services
{
    public class AuditEventConsumerHostedService : BackgroundService
    {
        private readonly IServiceProvider _serviceProvider;

        public AuditEventConsumerHostedService(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        protected override Task ExecuteAsync(CancellationToken stoppingToken)
        {
            Task.Run(() =>
            {
                using var scope = _serviceProvider.CreateScope();
                var consumer = scope.ServiceProvider.GetRequiredService<AuditEventConsumer>();
                consumer.StartConsuming();

                stoppingToken.WaitHandle.WaitOne();
                consumer.StopConsuming();
            }, stoppingToken);

            return Task.CompletedTask;
        }
    }
} 