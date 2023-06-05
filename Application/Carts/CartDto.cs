using Application.Common.Mappings;
using Application.Users;
using AutoMapper;
using Domain.Entities;

namespace Application.Carts
{
    public class CartDto : IMapFrom<Cart>
    {
        public int CustomerId { get; set; }

        public CustomerDto Customer { get; set; }

        public List<CartItemDto> CartItems { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Cart, CartDto>().ReverseMap();
        }
    }
}
