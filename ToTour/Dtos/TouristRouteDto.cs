namespace ToTour.Dtos
{
    public class TouristRouteDto
    {
        public Guid Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        //价格 = 原价 * 折扣
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }//
        public DateTime CreateTime { get; set; }
        public DateTime? UpDateTime { get; set; }
        public DateTime? DepartureTime { get; set; }
        public string Features { get; set; }
        public string Fees { get; set; }
        public string Notes { get; set; }
        public double? Rating { get; set; }  //评分
        public string TravelDays { get; set; }  //Model中的 枚举 改为 字符串
        public string TripType { get; set; }
        public string DepartureCity { get; set; }
        public ICollection<TouristRoutePictureDto> TouristRoutePictures { get; set; }
    }
}
