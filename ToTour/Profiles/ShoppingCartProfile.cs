using AutoMapper;
using ToTour.Dtos;
using ToTour.Models;

namespace ToTour.Profiles
{
    public class ShoppingCartProfile: Profile
    {
        public ShoppingCartProfile()
        {
            CreateMap<ShoppingCart, ShoppingCartDto>();
            CreateMap<LineItem, LineItemDto>();
        }
    }
}
