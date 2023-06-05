using Domain.Common;
using Domain.Enums;

namespace Domain.Entities;

public class DiscountProfile : AuditableEntity
{
    public DiscountType DiscountType { get; set; }
    public QualificationRequirement QualificationRequirement { get; set; }
    public double DiscountPercentage { get; set; }
}

public class QualificationRequirement
{
    public int Id { get; set; }
    public int? MinimumMonthsAsMember { get; set; }
    public double? MinimumAmountSpent { get; set; }
}