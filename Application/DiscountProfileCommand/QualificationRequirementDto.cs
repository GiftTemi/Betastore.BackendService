using Application.Common.Mappings;
using AutoMapper;
using Domain.Entities;

namespace Application.DiscountProfileCommand
{

    public class QualificationRequirementDto : IMapFrom<QualificationRequirement>
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int DurationInMonths { get; set; }
        public double MinimumAmountSpent { get; set; }
        public void Mapping(Profile profile)
        {
            profile.CreateMap<QualificationRequirement, QualificationRequirementDto>().ReverseMap();
        }
    }
}
