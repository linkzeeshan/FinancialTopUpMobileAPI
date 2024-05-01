using MobileTopUpAPI.Domain.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace MobileTopUpAPI.Application.Common.Models.Dtos
{
    public class BalanceCreateDto
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }
    public class BalanceReadDto
    {
        public int UserId { get; set; }
        public decimal Amount { get; set; }
    }
}
