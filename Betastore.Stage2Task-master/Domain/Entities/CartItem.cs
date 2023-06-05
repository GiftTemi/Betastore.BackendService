using Domain.Common;

namespace Domain.Entities;

public class CartItem : AuditableEntity
{
    public int CartId { get; set; }
    public Cart Cart { get; set; }
    public int ItemId { get; set; }
    public Item Item { get; set; }
    public int Quantity { get; set; }
}
