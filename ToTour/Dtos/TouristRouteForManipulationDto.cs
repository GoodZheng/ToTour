using System.ComponentModel.DataAnnotations;
using ToTour.Models;
using ToTour.ValidationAttributes;

namespace ToTour.Dtos
{
    [TouristRouteTitleMustBeDifferentFromDescription]
    public abstract class TouristRouteForManipulationDto
    {
        [Required(ErrorMessage = "title不能为空！")]
        [MaxLength(100)]
        public string Title { get; set; }
        [Required]
        [MaxLength(1500)]
        public virtual string Description { get; set; }
        //价格 = 原价 * 折扣
        public decimal Price { get; set; }
        //public decimal OriginalPrice { get; set; }
        //public double? DiscountPresent { get; set; }
        public DateTime CreateTime { get; set; }
        public DateTime? UpDateTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public string Features { get; set; }
        public string Fees { get; set; }
        public string Notes { get; set; }
        public double? Rating { get; set; }  //评分
        [EnumDataType(typeof(TravelDays), ErrorMessage = "TravelDays范围是0-8，分别代表One,Two,Three,Four,Five,Six,Seven,Eight,EightPlus")]
        public TravelDays TravelDays { get; set; }  //enum 枚举 
        [EnumDataType(typeof(TripType), ErrorMessage = $"TripType范围是0-4")]
        public TripType TripType { get; set; }
        [EnumDataType(typeof(DepartureCity), ErrorMessage = "DepartureCity的范围是0-3")]
        public DepartureCity DepartureCity { get; set; }
        public ICollection<TouristRoutePictureForCreationDto> TouristRoutePictures { get; set; }
            = new List<TouristRoutePictureForCreationDto>();
    }
}

