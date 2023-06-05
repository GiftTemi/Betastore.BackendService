using Application.Common.Mappings;
using AutoMapper;
using Domain.Common;
using Domain.Entities;
using Domain.Enums;

namespace Application.DiscountProfileCommand;
public class DiscountProfileDto : IMapFrom<DiscountProfile>
{
    public int Id { get; set; }
    public string Name { get; set; }
    public DiscountType DiscountType { get; set; }
    public QualificationRequirementDto QualificationRequirement { get; set; }
    public double DiscountPercentage { get; set; }
    public void Mapping(Profile profile)
    {
        profile.CreateMap<DiscountProfile, DiscountProfileDto>().ReverseMap();
    }
}
