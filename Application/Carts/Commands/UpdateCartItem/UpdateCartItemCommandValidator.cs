using FluentValidation;

namespace Application.Carts.Commands;
public class UpdateCartItemCommandValidator : AbstractValidator<UpdateCartItemCommand>
{
    public UpdateCartItemCommandValidator()
    {
        RuleFor(x => x.CartItemId).GreaterThan(0).WithMessage("Invalid cart item ID");
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity should be greater than 0");
        RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required");
    }
}
