using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ToTour.Models
{
    public class LineItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        [ForeignKey("TouristRouteId")]
        public Guid TouristRouteId { get; set; }
        
        public TouristRoute TouristRoute { get; set; }
        
        public Guid? ShoppingCartId { get; set; }
        
        //public Guid? Order { get; set; }
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalPrice { get; set; }
        
        [Range(0.0, 1.0)]
        public double? Discount { get; set; }
    }
}
