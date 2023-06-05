using Domain.Common;

namespace Domain.Entities;

public class Permission : AuditableEntity
{
    public List<RolePermission> RolePermissions { get; set; }

    public Permission()
    {
        RolePermissions = new List<RolePermission>();
    }
}