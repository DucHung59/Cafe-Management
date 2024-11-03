using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WNC.G06.Models
{
    [Table("tblOrders")]
    public class OrderModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }
        [Required]
        public float TotalAmount { get; set; }
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public Boolean Status { get; set; }

        [Required]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public UserModel User { get; set; }
        [Required]
        public int CafeID { get; set; }

        [ForeignKey("CafeID")]
        public CafeModel Cafe { get; set; }

        public ICollection<OrderDetailModel> OrderDetails { get; set; } = new List<OrderDetailModel>();
    }
}
