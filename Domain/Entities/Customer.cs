using Domain.Common;
using Domain.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Customer : AuditableEntity
{
    [ForeignKey("User")]
    public string UserId { get; set; }

    public User User { get; set; }

    public DateTime DateJoined => User?.CreatedDate ?? DateTime.MinValue;
    public double TotalAmountSpent { get; set; }
}



public class Item : AuditableEntity
{
    public string Description { get; set; }
    public double Price { get; set; }
    public int StockQuantity { get; set; }
}

public class Purchase : AuditableEntity
{
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    public string UserId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public List<PurchaseItem> PurchaseItems { get; set; }
    public bool DiscountApplied { get; set; } = false;
    public double DiscountAmount { get; set; } = 0;

    public double OriginalAmount
    {
        get
        {
            if (PurchaseItems == null || PurchaseItems.Count == 0)
                return 0;
            return PurchaseItems.Sum(item => item.Price * item.Quantity);
        }
    }
    public double PurchaseAmount
    {
        get
        {
            return OriginalAmount - DiscountAmount;
        }
    }
}

public class PurchaseItem : AuditableEntity
{
    public int? PurchaseId { get; set; }
    public Purchase? Purchase { get; set; }
    public int? ItemId { get; set; }
    public Item? Item { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
}

