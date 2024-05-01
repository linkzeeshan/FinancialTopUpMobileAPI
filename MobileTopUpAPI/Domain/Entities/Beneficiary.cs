using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileTopUpAPI.Domain.Entities
{
    [Table("Beneficiary", Schema = "dbo")]
    public class Beneficiary : BaseAuditableEntity<int>
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "Nickname is required")]
        [StringLength(20, ErrorMessage = "Beneficiary nickname cannot be longer than 20 characters")]
        public string Nickname { get; set; }

        public bool? IsActive { get; set; }

        [InverseProperty("Beneficiary")]
        public virtual ICollection<TopUpTransaction> TopUpTransactions { get; set; } = new List<TopUpTransaction>();

        [ForeignKey("UserId")]
        [InverseProperty("Beneficiaries")]
        public virtual User User { get; set; }
    }

}
