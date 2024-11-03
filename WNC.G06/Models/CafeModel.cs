using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WNC.G06.Models
{
    [Table("tblCafes")]
    public class CafeModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CafeID { get; set; }
        [Required]
        public string CafeName { get; set; }
        [Required]
        public string Address { get; set; }
        [Column("Phone", TypeName = "varchar(10)"), Required]
        public string Phone { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public UserModel User { get; set; }

        public ICollection<ProductModel> Products { get; set; } = new List<ProductModel>();
        public ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();

    }
}
