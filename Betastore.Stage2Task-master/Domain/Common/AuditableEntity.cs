using Domain.Enums;

namespace Domain.Common;

public abstract class AuditableEntity
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string? CreatedBy { get; set; }
    public string? CreatedById { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.Now;
    public string? LastModifiedBy { get; set; }
    public string? LastModifiedById { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public Status Status { get; set; } = Status.Active;
    public string StatusDesc => Status.ToString();
}
