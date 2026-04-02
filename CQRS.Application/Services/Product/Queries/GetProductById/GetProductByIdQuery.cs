using CQRS.Application.DTOs.Product;
using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Services.Product.Queries.GetProductById
{
    public record GetProductByIdQuery(int Id) : IRequest<ProductDto?>;
}
