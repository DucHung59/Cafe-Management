using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WNC.G06.Models
{
    [Table("tblPermissions")]
    public class PermissionsModel
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PermissionID { get; set; }

        [Required]
        public string Role { get; set; }

        public ICollection<UserModel> Users { get; set; } = new List<UserModel>();
    }
}
