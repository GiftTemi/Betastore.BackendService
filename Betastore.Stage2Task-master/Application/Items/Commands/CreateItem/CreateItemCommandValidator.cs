using FluentValidation;

namespace Application.Items.Commands;

public class CreateItemCommandValidator : AbstractValidator<CreateItemCommand>
{
    public CreateItemCommandValidator()
    {
        RuleFor(command => command.Name)
            .NotEmpty().WithMessage("Item name is required.")
            .MaximumLength(100).WithMessage("Item name must not exceed 100 characters.");

        RuleFor(command => command.Price)
            .GreaterThan(0).WithMessage("Price must be greater than zero.");
    }
}
