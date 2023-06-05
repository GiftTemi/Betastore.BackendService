using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using Domain.Enums;

namespace Application.Items.Queries;

public class ItemDto : IMapFrom<Item>
{
    public int Id { get; set; }
    public string? Name { get; set; }
    public string Description { get; set; }
    public double Price { get; set; }
    public int StockQuantity { get; set; }
    public string? CreatedBy { get; set; }
    public DateTime CreatedDate { get; set; }
    public string? LastModifiedBy { get; set; }
    public DateTime? LastModifiedDate { get; set; }
    public Status Status { get; set; }
    public string StatusDesc => Status.ToString();

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Item, ItemDto>().ReverseMap();
    }
}
