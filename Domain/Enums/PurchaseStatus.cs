using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Domain.Enums
{
    public enum PurchaseStatus
    {
        Pending = 1,
        Completed = 2,
        Shipped = 3,
        Delivered = 4,
        Canceled = 5,
        // Add more status values as needed
    }
}
