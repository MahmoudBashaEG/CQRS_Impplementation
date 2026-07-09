using CQRS.Application.Common;
using CQRS.Application.Events.Product;
using CQRS.Application.Interfaces.Brokers;
using CQRS.Application.Interfaces.Infrastructure;
using CQRS.Domain.EntityDomains;
using CQRS.Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;

namespace CQRS.Application.Services.Product.Commands.CreateProduct
{
    public class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, int>
    {
        private readonly IWriteDbContext _writeContext;

        public CreateProductCommandHandler(
            IWriteDbContext writeContext)
        {
            _writeContext = writeContext;
        }


        public async Task<int> Handle(CreateProductCommand request, CancellationToken cancellationToken)
        {
            using var transaction = await _writeContext.BeginTransactionAsync(cancellationToken);

            try
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


                var productCreatedEvent = new ProductCreatedEvent
                {
                    Id = product.Id,
                    Name = product.Name,
                    Stock = product.Stock,
                    UnitPrice = product.UnitPrice,
                    ProductStatus = product.ProductStatus,
                };
                var outboxMessage = OutboxMessage.BuildOutboxMessage<ProductCreatedEvent>(BrokerEventsEnum.ProductCreatedEvent, productCreatedEvent);

                _writeContext.OutboxMessages.Add(outboxMessage);

                await _writeContext.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);

                return product.Id;
            }
            catch (Exception)
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
            
        }
    }
}
