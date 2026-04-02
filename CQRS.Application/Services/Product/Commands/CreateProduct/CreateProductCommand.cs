using MediatR;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Services.Product.Commands.CreateProduct
{
    public record CreateProductCommand(string Name, int Stock, decimal UnitPrice) : IRequest<int>;
}
