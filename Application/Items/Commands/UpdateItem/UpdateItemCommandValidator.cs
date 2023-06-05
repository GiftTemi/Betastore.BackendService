using FluentValidation;

namespace Application.Items.Commands
{
    public class UpdateItemCommandValidator : AbstractValidator<UpdateItemCommand>
    {
        public UpdateItemCommandValidator()
        {
            RuleFor(x => x.ItemId).NotEmpty().WithMessage("Item ID is required.");
            RuleFor(x => x.UserId).NotEmpty().WithMessage("User ID is required.");
            RuleFor(x => x.Name).NotEmpty().WithMessage("Item name is required.");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
            RuleFor(x => x.Description).NotEmpty().WithMessage("Item description is required.");
            // Add validation rules for other item properties
        }
    }

}
