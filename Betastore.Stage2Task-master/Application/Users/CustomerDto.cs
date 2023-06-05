using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Application.Users
{
    public class CustomerDto : IMapFrom<Customer>
    {
        public string UserId { get; set; }

        public User User { get; set; }

        public DateTime DateJoined => User?.CreatedDate ?? DateTime.MinValue;
        public double TotalAmountSpent { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<Customer, CustomerDto>().ReverseMap();
        }
    }
}
