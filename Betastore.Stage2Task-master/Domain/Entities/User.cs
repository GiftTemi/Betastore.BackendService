using Microsoft.AspNetCore.Identity;
using Domain.Enums;
using Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace Domain.Entities;

public class User : IdentityUser
{
    public string? FirstName { get; set; }
    public string? Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? LastName { get; set; }
    public DateTime LastLoginDate { get; set; }
    public AccessLevel AccessLevel { get; set; }
    [ForeignKey("Role")]
    public int? RoleId { get; set; }
    public Role? Role { get; set; }
    public string? Password { get; set; }
    public DateTime CreatedDate { get; set; }
    public Status Status { get; set; } = Status.Active;
    public string? StatusDesc => Status.ToString();
    public string? Name => string.Concat(FirstName + " " + MiddleName ?? " " + " " + LastName ?? " ");
    public string? Token { get; set; }
    public string? Otp { get; set; }
    public string? CreatedBy { get;  set; }
    public string? UserId => Id.ToString();
    public string? LastModifiedBy { get;  set; }
    public DateTime? LastModifiedDate { get;  set; }
    public string? MiddleName { get;  set; }
}