using CQRS.Application.Common;
using CQRS.Application.Events.BrokerEvents;
using CQRS.Application.Events.BrokerEvents.Product;
using CQRS.Application.Interfaces.Brokers;
using CQRS.Application.Interfaces.Infrastructure;
using CQRS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Services.Product.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IWriteDbContext _writeContext;
        private readonly IBrokerProducer _producerBroker;

        public CreateProductCommandHandler(
            IWriteDbContext writeContext,
            IBrokerProducer producerBroker)
        {
            _writeContext = writeContext;
            _producerBroker = producerBroker;
        }


        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            var product = new CQRS.Domain.EntityDomains.Product
            {
                Name = request.Name,
                Stock = request.Stock,
                UnitPrice = request.UnitPrice,
                ProductStatus = ProductStatuses.Draft,
            };

            _writeContext.Products.Add(product);
            await _writeContext.SaveChangesAsync(cancellationToken);

            var eventt = new ProductCreatedEvent
            {
                Id = product.Id,
                Name = product.Name,
                Stock = product.Stock,
                UnitPrice = product.UnitPrice,
                ProductStatus = product.ProductStatus,
            };

            await _producerBroker.PublishAsync(eventt, BrokerEventsEnum.ProductCreatedEvent);
            return product.Id;
        }
    }
}
