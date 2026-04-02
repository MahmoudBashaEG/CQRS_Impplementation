using CQRS.Application.Common;
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
        private readonly IReadDbContext _readContext;

        public CreateProductCommandHandler(
            IWriteDbContext writeContext, 
            IReadDbContext readContext)
        {
            _writeContext = writeContext;
            _readContext = readContext;
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

            if (Constants.IsReadWriteNotSameDatabase)
            {
                _readContext.Products.Add(product);
                product.Id = 0;
                await _readContext.SaveChangesAsync(cancellationToken);
            }
            return product.Id;
        }
    }
}
