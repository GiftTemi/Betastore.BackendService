using Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class Cart : AuditableEntity
{
    [ForeignKey("Customer")]
    public int CustomerId { get; set; }

    public Customer Customer { get; set; }

    public List<CartItem> CartItems { get; set; }
}
