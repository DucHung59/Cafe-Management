using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Security;

namespace WNC.G06.Models
{
    [Table("tblUsers")]
    public class UserModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserID { get; set; }
        [Required]
        public string UserName { get; set; }
        [Column("Password", TypeName = "varchar(max)"), Required]
        public string Password { get; set; }
        [Column("Email", TypeName = "varchar(max)"), Required]
        public string Email { get; set; }
        [Required]
        public Boolean Status { get; set; }
        [Required]
        public int PermissionID { get; set; }

        [ForeignKey("PermissionID")]
        public PermissionsModel Permission { get; set; }
        public ICollection<CafeModel> Cafes { get; set; } = new List<CafeModel>();
        public ICollection<OrderModel> Orders { get; set; } = new List<OrderModel>();
        public ICollection<PaymentModel> Payments { get; set; } = new List<PaymentModel>();
    }
}
