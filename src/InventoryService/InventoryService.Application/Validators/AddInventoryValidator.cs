using FluentValidation;
using InventoryService.Application.DTOs;

namespace InventoryService.Application.Validators;

public class AddInventoryValidator : AbstractValidator<AddInventoryDto>
{
    public AddInventoryValidator()
    {
        RuleFor(x => x.ProductName)
            .NotEmpty().WithMessage("Product name is required")
            .MaximumLength(200).WithMessage("Product name cannot exceed 200 characters");

        RuleFor(x => x.Quantity)
            .GreaterThanOrEqualTo(0).WithMessage("Quantity cannot be negative");

        RuleFor(x => x.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero");
    }
}