using CQRS.Application.Events;
using CQRS.Application.Interfaces.Brokers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Infrastructure.BackgroundServices
{

    public class OutboxConsumerBackgroundService : BackgroundService
    {
        private readonly IBrokerConsumer _consumer;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OutboxConsumerBackgroundService> _logger;

        public OutboxConsumerBackgroundService(
            IBrokerConsumer consumer,
            IServiceScopeFactory scopeFactory,
            ILogger<OutboxConsumerBackgroundService> logger)
        {
            _consumer = consumer;
            _scopeFactory = scopeFactory;
            _logger = logger;
        }


        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            await _consumer.StartConsumingAsync(
                HandleMessageAsync,
                stoppingToken);
        }

        private async Task HandleMessageAsync(BrokerMessageEnvelope message, CancellationToken cancellationToken)
        {
            using var scope = _scopeFactory.CreateScope();

            var handlers = scope.ServiceProvider
                .GetRequiredService<IEnumerable<IBrokerEventHandler>>();

            var handler = handlers.FirstOrDefault(x => x.EventType == message.EventType);

            if (handler is null)
            {
                throw new InvalidOperationException(
                    $"No handler registered for event type {message.EventType}.");
            }

            await handler.HandleAsync(message, cancellationToken);

            _logger.LogInformation(
                "RabbitMQ message {MessageId} handled successfully by {HandlerName}. EventType: {EventType}",
                message.MessageId,
                handler.GetType().Name,
                message.EventType);
        }
    }
}
