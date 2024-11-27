using System.ComponentModel.DataAnnotations;
using ToTour.Dtos;

namespace ToTour.ValidationAttributes
{
    //对Dto进行数据验证
    public class TouristRouteTitleMustBeDifferentFromDescriptionAttribute:ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext) //参数：输入的数据对象、验证的上下文关系对象
        {
            var touristRouteDto = (TouristRouteForManipulationDto)validationContext.ObjectInstance;
            if (touristRouteDto.Title == touristRouteDto.Description)
            {
                return new ValidationResult("title不能与Description相同！", new[] { "TouristRouteForCreationDto" });
            }
            return ValidationResult.Success;
        }
    }
}
