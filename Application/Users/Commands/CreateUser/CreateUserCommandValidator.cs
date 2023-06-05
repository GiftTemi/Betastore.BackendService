using FluentValidation;

namespace Application.Users.Commands
{
    public class CreateUserCommandValidator : AbstractValidator<CreateUserCommand>
    {
        public CreateUserCommandValidator()
        {
            RuleFor(v => v.FirstName)
                 .NotEmpty();

            RuleFor(v => v.LastName)
                .NotEmpty();

            RuleFor(v => v.Email)
            .NotEmpty();

        }
    }
}
