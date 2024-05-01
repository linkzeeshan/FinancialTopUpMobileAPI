using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileTopUpAPI.Domain.Entities
{
    [Table("Balance", Schema = "dbo")]
    public class Balance : BaseAuditableEntity<int>
    {
        public int UserId { get; set; }

        [Column(TypeName = "decimal(18, 0)")]
        public decimal Amount { get; set; }

        public bool? IsActive { get; set; } = true;

        [ForeignKey("UserId")]
        [InverseProperty("Balances")]
        public virtual User User { get; set; }
    }
}
