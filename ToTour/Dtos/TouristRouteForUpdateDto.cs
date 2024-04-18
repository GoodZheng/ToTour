using ToTour.Models;
using System.ComponentModel.DataAnnotations;
using ToTour.ValidationAttributes;

namespace ToTour.Dtos
{
    
    public class TouristRouteForUpdateDto:TouristRouteForManipulationDto
    {
        [Required(ErrorMessage ="更新必须添加描述！")]
        [MaxLength(1500)]
        public override string Description { get; set; }
    }
}
