using Domain.Common;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace Domain.Entities
{
    public class ApplicationRole : IdentityRole
    {
    }
    public class Role : AuditableEntity
    {
        public ICollection<RolePermission> RolePermissions { get; set; }
    }
}
