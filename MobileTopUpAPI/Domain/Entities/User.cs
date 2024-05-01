using Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileTopUpAPI.Domain.Entities
{
    [Table("User", Schema = "dbo")]
    public class User : BaseAuditableEntity<int>
    {
        [StringLength(250)]
        [Column("UserName")]
        public string? UserName { get; set; }
        [Column("PhoneNumber")]
        [StringLength(50)]
        public string PhoneNumber { get; set; }
        [Column("IsVerified")]
        public bool IsVerified { get; set; } = true;

        // Other user properties
        [InverseProperty("User")]
        public virtual ICollection<Balance> Balances { get; set; } = new List<Balance>();

        [InverseProperty("User")]
        public virtual ICollection<Beneficiary> Beneficiaries { get; set; } = new List<Beneficiary>();

        [InverseProperty("User")]
        public virtual ICollection<TopUpTransaction> TopUpTransactions { get; set; } = new List<TopUpTransaction>();
    }

}
