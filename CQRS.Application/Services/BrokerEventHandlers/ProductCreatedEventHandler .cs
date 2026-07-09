using CQRS.Application.Events;
using CQRS.Application.Events.Product;
using CQRS.Application.Interfaces.Brokers;
using CQRS.Application.Interfaces.Infrastructure;
using CQRS.Domain.EntityDomains;
using CQRS.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CQRS.Application.Services.BrokerEventHandlers
{
    public sealed class ProductCreatedEventHandler : IBrokerEventHandler
    {
        private readonly IReadDbContext _dbContext;

        public ProductCreatedEventHandler(IReadDbContext readDbContext)
        {
            _dbContext = readDbContext;
        }

        public BrokerEventsEnum EventType => BrokerEventsEnum.ProductCreatedEvent;

        public async Task HandleAsync(
            BrokerMessageEnvelope message,
            CancellationToken cancellationToken)
        {
            var handlerName = nameof(ProductCreatedEventHandler);

            await using var transaction =
                await _dbContext.BeginTransactionAsync(cancellationToken);

            var alreadyProcessed = await _dbContext.InboxMessages
                .AnyAsync(
                    x => x.MessageId == message.MessageId &&
                         x.HandlerName == handlerName,
                    cancellationToken);

            if (alreadyProcessed)
            {
                await transaction.CommitAsync(cancellationToken);
                return;
            }

            var productCreatedEvent =
                JsonSerializer.Deserialize<ProductCreatedEvent>(
                    message.Payload);

            if (productCreatedEvent is null)
            {
                throw new InvalidOperationException(
                    "Failed to deserialize ProductCreatedIntegrationEvent.");
            }

            var product = await _dbContext.Products
                .FirstOrDefaultAsync(
                    x => x.Id == productCreatedEvent.Id,
                    cancellationToken);

            if (product is null)
            {
                product = new Domain.EntityDomains.Product
                {
                    Id = productCreatedEvent.Id,
                    Name = productCreatedEvent.Name,
                    Stock = productCreatedEvent.Stock,
                    UnitPrice = productCreatedEvent.UnitPrice,
                    ProductStatus = productCreatedEvent.ProductStatus,
                };

                _dbContext.Products.Add(product);
            }
            else
            {
                product.Name = productCreatedEvent.Name;
                product.Stock = productCreatedEvent.Stock;
                product.UnitPrice = productCreatedEvent.UnitPrice;
                product.ProductStatus = productCreatedEvent.ProductStatus;
            }

            _dbContext.InboxMessages.Add(new InboxMessage
            {
                MessageId = message.MessageId,
                HandlerName = handlerName,
                ProcessedOnUtc = DateTime.UtcNow
            });

            await _dbContext.SaveChangesAsync(cancellationToken);

            await transaction.CommitAsync(cancellationToken);
        }
    }
}
