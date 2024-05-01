using Application.Common.Models.Dtos;
using MobileTopUpAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations;

namespace MobileTopUpAPI.Application.Common.Models.Dtos
{
    public class TopUpTransactionCreateDto
    {
        public int UserId { get; set; }
        public int BeneficiaryId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public decimal Charge { get; set; }
        [Required]
        public decimal TotalAmount { get; set; }
        public DateTime TransactionDate { get; set; }  
    }
    public class TopUpTransactionRequest
    {
        public int UserId { get; set; }
        public int BeneficiaryId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        public DateTime TransactionDate { get; set; }
    }
    public class TopUpTransactionReadDto
    {
        public int UserId { get; set; }
        public int BeneficiaryId { get; set; }
        public decimal Amount { get; set; }
        public decimal Charge { get; set; }
        public decimal TotalAmount { get; set; }
        public DateTime TransactionDate { get; set; } = new DateTime();
        public User User { get; set; } = new User();
        public Beneficiary Beneficiary { get; set; } = new Beneficiary();
    }
    public class TopUpOptionDTO
    {
        public int Key { get; set; }
        public decimal Amount { get; set; }
    }
}
