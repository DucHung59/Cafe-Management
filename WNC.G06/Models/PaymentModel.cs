using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WNC.G06.Models
{
    [Table("tblPayments")]
    public class PaymentModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentID { get; set; }
        [Required]
        public float Amount { get; set; }
        [Required]
        public DateTime Date { get; set; }
        [Required]
        public int UserID { get; set; }

        [ForeignKey("UserID")]
        public UserModel User { get; set; }
    }
}
