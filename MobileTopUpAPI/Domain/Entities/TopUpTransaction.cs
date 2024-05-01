using Domain.Common;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileTopUpAPI.Domain.Entities
{
    [Table("TopUpTransaction", Schema = "dbo")]
    public class TopUpTransaction : BaseAuditableEntity<int>
    {
        [ForeignKey(nameof(TopUpTransaction))]
        [Column("UserId")]
        public int UserId { get; set; }

        [Column("BeneficiaryId")]
        public int BeneficiaryId { get; set; }

        [Column("Amount", TypeName = "decimal(18, 0)")]
        public decimal Amount { get; set; }
        [Column("Charge")]
        public decimal? Charge { get; set; }
        [Column("TotalAmount", TypeName = "decimal(18, 0)")]
        public decimal TotalAmount { get; set; }
        [Column("TransactionDate")]
        public DateTime? TransactionDate { get; set; } = new DateTime();

        [ForeignKey("BeneficiaryId")]
        [InverseProperty("TopUpTransactions")]
        public virtual Beneficiary Beneficiary { get; set; }

        [ForeignKey("UserId")]
        [InverseProperty("TopUpTransactions")]
        public virtual User User { get; set; }
    }

}
