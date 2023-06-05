using FluentValidation;

namespace Application.Users.Commands
{
    public class ChangeUserStatusCommandValidator : AbstractValidator<ChangeUserStatusCommand>
    {
        public ChangeUserStatusCommandValidator()
        {
            RuleFor(v => v.UserId)
                .NotEmpty();
        }
    }
}
