using CQRS.Application.Interfaces.Brokers;
using CQRS.Application.Interfaces.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Infrastructure.BackgroundServices
{
    public sealed class OutboxPublisherBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IBrokerProducer _publisher;
        private readonly ILogger<OutboxPublisherBackgroundService> _logger;

        public OutboxPublisherBackgroundService(
            IServiceScopeFactory scopeFactory,
            IBrokerProducer publisher,
            ILogger<OutboxPublisherBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _publisher = publisher;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                await PublishPendingOutboxMessagesAsync(stoppingToken);

                await Task.Delay(TimeSpan.FromSeconds(5), stoppingToken);
            }
        }

        private async Task PublishPendingOutboxMessagesAsync(
            CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var dbContext = scope.ServiceProvider.GetRequiredService<IWriteDbContext>();

            var messages = await dbContext.OutboxMessages
                .Where(x => x.ProcessedOnUtc == null)
                .OrderBy(x => x.OccurredOnUtc)
                .Take(20)
                .ToListAsync(cancellationToken);

            if (messages.Count == 0)
                return;

            foreach (var message in messages)
            {
                try
                {
                    await _publisher.PublishAsync(message, cancellationToken);

                    message.ProcessedOnUtc = DateTime.UtcNow;
                    message.Error = null;
                }
                catch (Exception ex)
                {
                    message.RetryCount++;
                    message.Error = ex.Message;

                    _logger.LogError(
                        ex,
                        "Failed to publish outbox message {MessageId} with event type {EventType}",
                        message.Id,
                        message.EventType);
                }
            }

            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}
