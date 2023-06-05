using Domain.Enums;

namespace Domain.Entities;

public class RolePermission
{
    public int Id { get; set; }
    public int RoleId { get; set; }
    public Role Role { get; set; }
    public int PermissionId { get; set; }
    public Permission Permission { get; set; }
    public Status Status { get; set; } = Status.Active;
}
