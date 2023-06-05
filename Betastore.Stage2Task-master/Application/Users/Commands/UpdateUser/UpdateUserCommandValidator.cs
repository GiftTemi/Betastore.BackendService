using FluentValidation;

namespace Application.Users.Commands
{
    public class UpdateUserCommandValidator : AbstractValidator<UpdateUserCommand>
    {
        public UpdateUserCommandValidator()
        {
            RuleFor(v => v.FirstName)
                .MaximumLength(200)
                .NotEmpty().WithMessage("Invalid First Name Details");

            RuleFor(v => v.LastName)
                .MaximumLength(200)
                .NotEmpty().WithMessage("Invalid Last Name Details");

        }
    }
}
