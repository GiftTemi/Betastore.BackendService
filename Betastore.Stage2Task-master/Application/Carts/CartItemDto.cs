using Application.Carts;
using Application.Common.Mappings;
using Application.Items.Queries;
using AutoMapper;
using Domain.Entities;

namespace Application;
public class CartItemDto : IMapFrom<CartItem>
{
    public int CartId { get; set; }
    public CartDto Cart { get; set; }
    public int ItemId { get; set; }
    public ItemDto Item { get; set; }
    public int Quantity { get; set; }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<CartItem, CartItemDto>().ReverseMap();
    }
}

