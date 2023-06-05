using System;
using System.Collections.Generic;
using System.Text;

namespace Domain.Constants
{
    public class DefaultPermissions
    {
        public static List<string> GeneratePermissionsForModule(string module)
        {
            return new List<string>()
        {
            $"Add {module}",
            $"View {module}",
            $"Edit {module}",
            $"Delete {module}",
        };
        }

        public static class Customer
        {
            // Customers
            public const string View = "Permissions.Customer.View";
            public const string Create = "Permissions.Customer.Create";
            public const string Edit = "Permissions.Customer.Edit";
            public const string Disable = "Permissions.Customer.Disable";
            public const string Delete = "Permissions.Customer.Delete";
        }

        public static class Product
        {
            // Products
            public const string View = "Permissions.Product.View";
            public const string Create = "Permissions.Product.Create";
            public const string Edit = "Permissions.Product.Edit";
            public const string Delete = "Permissions.Product.Delete";
        }

        public static class Order
        {
            // Orders
            public const string View = "Permissions.Order.View";
            public const string Create = "Permissions.Order.Create";
            public const string Edit = "Permissions.Order.Edit";
            public const string Cancel = "Permissions.Order.Cancel";
        }

        public static class Payment
        {
            // Payments
            public const string View = "Permissions.Payment.View";
            public const string Process = "Permissions.Payment.Process";
            public const string Refund = "Permissions.Payment.Refund";
            public const string Void = "Permissions.Payment.Void";
        }

        public static class Shipping
        {
            // Shipments
            public const string View = "Permissions.Shipping.View";
            public const string Create = "Permissions.Shipping.Create";
            public const string Edit = "Permissions.Shipping.Edit";
            public const string Cancel = "Permissions.Shipping.Cancel";
        }

        public static class Reporting
        {
            // Reporting and Analytics
            public const string View = "Permissions.Reporting.View";
            public const string Generate = "Permissions.Reporting.Generate";
            public const string Export = "Permissions.Reporting.Export";
            public const string Analyze = "Permissions.Reporting.Analyze";
        }

        public static class Support
        {
            // Customer Support
            public const string View = "Permissions.Support.View";
            public const string Create = "Permissions.Support.Create";
            public const string Resolve = "Permissions.Support.Resolve";
            public const string Close = "Permissions.Support.Close";
        }
    }
}