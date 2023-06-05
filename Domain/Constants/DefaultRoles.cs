using System.Reflection;

namespace Domain.Constants;

public class DefaultRoles
{
    public const string SuperAdmin = "SuperAdmin";
    public const string Admin = "Admin";
    public const string CustomerSupport = "CustomerSupport";
    public const string Customer = "Customer";

    public static class CustomRoles
    {
        public const string SalesManager = "SalesManager";
        public const string WarehouseManager = "WarehouseManager";
    }
}

