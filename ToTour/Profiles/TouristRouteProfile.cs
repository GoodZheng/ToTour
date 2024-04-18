using AutoMapper;
using ToTour.Dtos;
using ToTour.Models;

namespace ToTour.Profiles
{
    public class TouristRouteProfile:Profile
    {
        public TouristRouteProfile()
        {
            CreateMap<TouristRoute, TouristRouteDto>()
                .ForMember(
                    dest => dest.Price,
                    opt => opt.MapFrom(src => src.OriginalPrice * (decimal)(src.DiscountPresent ?? 1))
                )
                .ForMember(
                    dest => dest.TravelDays,
                    opt => opt.MapFrom(src => src.TravelDays.ToString())
                )
                .ForMember(
                    dest => dest.TripType,
                    opt => opt.MapFrom(src => src.TripType.ToString())
                )
                .ForMember(
                    dest => dest.DepartureCity,
                    opt => opt.MapFrom(src => src.DepartureCity.ToString())
                );

            CreateMap<TouristRouteForCreationDto, TouristRoute>()
                .ForMember(
                    dest => dest.Id, //目标对象
                    opt => opt.MapFrom(src => Guid.NewGuid()) //数据源
                );
            //.ForMember(
            //    dest => dest.TravelDays,
            //    opt => opt.MapFrom(src => (TravelDays)Enum.Parse(typeof(TravelDays), src.TravelDays)) //把三个string转换为枚举
            //)
            //.ForMember(
            //    dest => dest.TripType,
            //    opt => opt.MapFrom(src => (TripType)Enum.Parse(typeof(TripType), src.TripType))
            //)
            //.ForMember(
            //    dest => dest.DepartureCity,
            //    opt => opt.MapFrom(src => (DepartureCity)Enum.Parse(typeof(DepartureCity), src.DepartureCity))
            //);

            CreateMap<TouristRouteForUpdateDto, TouristRoute>();

            CreateMap<TouristRoute, TouristRouteForUpdateDto>();
        }
    }
}
