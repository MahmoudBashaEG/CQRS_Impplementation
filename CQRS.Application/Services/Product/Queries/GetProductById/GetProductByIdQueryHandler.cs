using CQRS.Application.DTOs.Product;
using CQRS.Application.Interfaces.Infrastructure;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Services.Product.Queries.GetProductById
{
    public class GetProductHandler : IRequestHandler<GetProductByIdQuery, ProductDto?>
    {
        private readonly IReadDbContext _context;

        public GetProductHandler(IReadDbContext context) => _context = context;

        public async Task<ProductDto?> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
        {
            var product = await _context.Products
                .Where(x => x.Id == request.Id)
                .Select(x => new ProductDto
                {
                    Id = x.Id,
                    Name = x.Name,
                    InStock = x.Stock != 0,
                })
                .FirstOrDefaultAsync(cancellationToken);

            return product;
        }
    }
}
