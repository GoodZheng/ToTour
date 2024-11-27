using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace ToTour.Models
{
    public class LineItem
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)] //声明为自增主键
        public int Id { get; set; }
        
        [ForeignKey("TouristRouteId")]
        public Guid TouristRouteId { get; set; }
        
        public TouristRoute TouristRoute { get; set; }
        
        public Guid? ShoppingCartId { get; set; } // 第二个外键
        
        //public Guid? Order { get; set; } // 第三个外键
        
        [Column(TypeName = "decimal(18,2)")]
        public decimal OriginalPrice { get; set; }
        
        [Range(0.0, 1.0)]
        public double? Discount { get; set; }
    }
}
