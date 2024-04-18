using AutoMapper;
using ToTour.Dtos;
using ToTour.Models;

namespace ToTour.Profiles
{
    public class TouristRoutePictureProfile : Profile
    {
        public TouristRoutePictureProfile()
        {
            CreateMap<TouristRoutePicture, TouristRoutePictureDto>(); //第一个类型：输入的原始类型（数据模型）   第二个类型：输出的目标类型（DTO对象）
            
            CreateMap<TouristRoutePictureForCreationDto, TouristRoutePicture>();

            CreateMap<TouristRoutePicture, TouristRoutePictureForCreationDto>();
        }
    }

}
