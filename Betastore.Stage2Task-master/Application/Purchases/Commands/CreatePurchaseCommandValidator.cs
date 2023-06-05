using FluentValidation;

namespace Application.Purchases.Commands;

public class CreatePurchaseCommandValidator : AbstractValidator<CreatePurchaseCommand>
{
    public CreatePurchaseCommandValidator()
    {
        RuleFor(command => command.CartId)
            .NotEmpty()
            .GreaterThan(0)
            .WithMessage("CartId must be greater than zero and not null.");

        RuleFor(command => command.UserId)
            .NotEmpty()
            .WithMessage("UserId must not be null or whitespace.");

        RuleFor(command => command.DisountProfileName)
            .NotEmpty()
            .When(command => command.DiscountApplied.HasValue && command.DiscountApplied.Value)
            .WithMessage("DisountProfileName must not be null when DiscountApplied is true.");
    }
}