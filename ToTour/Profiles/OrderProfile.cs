using AutoMapper;
using ToTour.Dtos;
using ToTour.Models;

namespace ToTour.Profiles
{
    public class OrderProfile : Profile
    {
        public OrderProfile()
        {
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State.ToString()));
        }
    }
}
