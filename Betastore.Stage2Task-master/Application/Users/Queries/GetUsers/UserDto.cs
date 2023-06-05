using AutoMapper;
using Application.Common.Mappings;
using Domain.Entities;
using Domain.Enums;

namespace Application.Users.Queries;

public class UserDto : IMapFrom<User>
{
    public string Id { get; set; }
    public string? FirstName { get; set; }
    public string? Email { get; set; }
    public string? Address { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string? LastName { get; set; }
    public DateTime LastLoginDate { get; set; }
    public AccessLevel AccessLevel { get; set; }
    public string AccessLevelDesc => AccessLevel.ToString();
    public int? RoleId { get; set; }
    public Role? Role { get; set; }
    public DateTime CreatedDate { get; set; }
    public Status Status { get; set; } = Status.Active;
    public string StatusDesc => Status.ToString();
    public string? Name => string.Concat(FirstName + " " + MiddleName ?? " " + " " + LastName ?? " ");
    public string? Token { get; set; }
    public string? Otp { get; set; }
    public string CreatedBy { get; set; }
    public string UserId { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public string MiddleName { get; set; }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<User, UserDto>().ReverseMap();
    }
}
