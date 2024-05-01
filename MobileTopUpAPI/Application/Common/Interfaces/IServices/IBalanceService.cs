using Application.Common.Models;
using Application.Common.Models.Dtos;
using MobileTopUpAPI.Application.Common.Models.Dtos;

namespace MobileTopUpAPI.Application.Common.Interfaces.IServices
{
    public interface IBalanceService
    {
        Task<ApiResponse<ApiResponse<BalanceReadDto>>> Add(BalanceCreateDto BalanceCreateDto);
        Task<ApiResponse<BalanceReadDto>> CreditOrUpdate(BalanceCreateDto balanceCreateDto);
        Task<ApiResponse<bool>> Delete(int id);
        Task<PaginatedList<BalanceReadDto>> GetAll(int pageNumber, int pageSize);
        Task<ApiResponse<BalanceReadDto>> GetById(int id);

        Task<decimal> GetBalanceByUserIdAsync(int userId);
        Task<bool> DebitBalanceByUserIdAsync(int userId, decimal amount);
    }
}
