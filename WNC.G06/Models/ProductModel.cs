using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WNC.G06.Models
{
    [Table("tblProducts")]
    public class ProductModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductID { get; set; }
        [Required]
        public string ProductName { get; set; }
        [Required]
        public float Price { get; set; }
        [Column("imgUrl", TypeName = "varchar(max)"), Required]
        public string imgUrl {get; set;}
        [Column("Description", TypeName = "varchar(max)"), Required]
        public string Description { get; set; }
        [Required]
        public Boolean Status { get; set; }
        [Required]
        public int CafeID { get; set; }

        [ForeignKey("CafeID")]
        public CafeModel Cafe { get; set; }
        public ICollection<OrderDetailModel> OrderDetails { get; set; } = new List<OrderDetailModel>();

    }
}
