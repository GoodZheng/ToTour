using System.ComponentModel.DataAnnotations;
using ToTour.Models;
using ToTour.ValidationAttributes;

namespace ToTour.Dtos
{
    [TouristRouteTitleMustBeDifferentFromDescription]
    public class TouristRouteForCreationDto:TouristRouteForManipulationDto
    {

    }
}
