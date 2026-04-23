using CQRS.Application.Events.BrokerEvents;
using CQRS.Application.Events.BrokerEvents.Product;
using CQRS.Application.Interfaces.Brokers;
using CQRS.Application.Interfaces.Infrastructure;
using CQRS.CrossCutting.Configurations;
using CQRS.Domain.EntityDomains;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CQRS.Application.Events.EventHandlers
{
    public class ProductCreatedEventHandler : IBrokerEventHandler
    {
        public BrokerEventsEnum BrokerEventType { get => BrokerEventsEnum.ProductCreatedEvent; }


        public async Task Handle(IServiceScopeFactory serviceScopeFactory, ScalingConfigurations scalingConfigurations, string jsonMessage)
        {
            if(scalingConfigurations.IsReadingAndWritingTheSameDatabase)
                return;

            using var scope = serviceScopeFactory.CreateScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IReadDbContext>();

            var productCreatedEvent = JsonSerializer.Deserialize<ProductCreatedEvent>(jsonMessage);
            if(productCreatedEvent == null)
                throw new InvalidCastException($"Invalid Deserialization at {nameof(ProductCreatedEventHandler.Handle)}... Failed to Deserialized to {nameof(ProductCreatedEvent)}");
            
            dbContext.Products.Add(new Product
            {
                Id = productCreatedEvent.Id,
                Name = productCreatedEvent.Name,
                ProductStatus = productCreatedEvent.ProductStatus,
                Stock = productCreatedEvent.Stock,
                UnitPrice = productCreatedEvent.UnitPrice
            });
        }
    }
}
