using Application.Context;
using Domain.Common.Models;
using Domain.Enums;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.DiscountProfileCommand.Commands
{
    public class ApplyApplicableDiscountCommand : IRequest<Result>
    {
        public int CustomerId { get; set; }
        public string UserId { get; set; }
    }

    public class ApplyApplicableDiscountCommandHandler : IRequestHandler<ApplyApplicableDiscountCommand, Result>
    {
        private readonly IApplicationDbContext _context;

        public ApplyApplicableDiscountCommandHandler(IApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Result> Handle(ApplyApplicableDiscountCommand request, CancellationToken cancellationToken)
        {
            // Retrieve the customer's profile
            var customer = await _context.Customers.FindAsync(request.CustomerId);
            if (customer == null)
            {
                Result.Failure("Customer not found");
            }

            // Retrieve the best applicable discount for the customer
            var discount = await _context.DiscountProfiles
                .Where(d =>
                //DiscountType is MembershipDuration and the customer's membership duration is greater or equal to the Configured Membership Duration
                (d.DiscountType == DiscountType.MembershipDuration && CalculateMembershipDuration(customer.DateJoined) >= d.QualificationRequirement.MinimumMonthsAsMember)

                //DiscountType is AmountSpent and the customer's TotalAmountSpent is greater or equal to the Configured MinimumAmountSpent
                || (d.DiscountType == DiscountType.AmountSpent && customer.TotalAmountSpent >= d.QualificationRequirement.MinimumAmountSpent))

                //Pick the best of the two
                .OrderByDescending(d => d.DiscountPercentage)
                .FirstOrDefaultAsync();

            if (discount == null)
            {
                return Result.Failure("No applicable discount found.");
            }

            // Apply the discount to the customer
            double discountAmount = (discount.DiscountPercentage / 100) * customer.TotalAmountSpent;

            // Update the customer's profile or perform any other necessary actions based on your business logic
            // For example, you can update the TotalAmountSpent property with the discounted amount
            customer.TotalAmountSpent -= discountAmount;

            // Save the changes
            await _context.SaveChangesAsync(cancellationToken);

            return Result.Success(new
            {
                discount = discount,
                discountOnTotalAmountSpent = discountAmount,
            });
        }

        private int CalculateMembershipDuration(DateTime dateJoined)
        {
            var today = DateTime.Today;
            var monthsAsMember = (today.Year - dateJoined.Year) * 12 + today.Month - dateJoined.Month;
            return monthsAsMember;
        }
    }

}
