using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.Purchases;

public class PurchaseDto : IMapFrom<Purchase>
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public string UserId { get; set; }
    public DateTime PurchaseDate { get; set; }
    public List<PurchaseItemDto> PurchaseItems { get; set; }

    public void Mapping(Profile profile)
    {
        profile.CreateMap<Purchase, PurchaseDto>().ReverseMap();
    }
}

public class PurchaseItemDto : IMapFrom<PurchaseItem>
{
    public int ItemId { get; set; }
    public int Quantity { get; set; }
    public double Price { get; set; }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<PurchaseItem, PurchaseItemDto>().ReverseMap();
    }
}