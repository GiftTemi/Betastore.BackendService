using FluentValidation;

namespace Application.Items.Queries
{
    public class GetItemByIdQueryValidator : AbstractValidator<GetItemByIdQuery>
    {
        public GetItemByIdQueryValidator()
        {
            RuleFor(query => query.ItemId).NotEmpty().WithMessage("ItemId is required.");
            RuleFor(query => query.UserId).NotEmpty().WithMessage("UserId is required.");
        }
    }

}
