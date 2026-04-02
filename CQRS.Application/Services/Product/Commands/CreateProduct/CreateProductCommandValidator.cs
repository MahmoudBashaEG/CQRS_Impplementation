using FluentValidation;
using System;
using System.Collections.Generic;
using System.Text;

namespace CQRS.Application.Services.Product.Commands.CreateProduct
{
    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(v => v.Name)
                .NotEmpty().WithMessage("Product name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(v => v.Stock)
                .GreaterThanOrEqualTo(0).WithMessage("Stock cannot be negative.");

            RuleFor(v => v.UnitPrice)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");
        }
    }
}
