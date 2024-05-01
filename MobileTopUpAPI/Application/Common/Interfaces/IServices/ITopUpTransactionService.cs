using Application.Common.Models;
using Application.Common.Models.Dtos;
using MobileTopUpAPI.Application.Common.Models.Dtos;

namespace MobileTopUpAPI.Application.Common.Interfaces.IServices
{
    public interface ITopUpTransactionService
    {
        Task<ApiResponse<TopUpTransactionReadDto>> CreateTopUpTransactionHistory(TopUpTransactionRequest TopUpTransactionCreateDto);
        Task<PaginatedList<TopUpTransactionReadDto>> GetAll(int pageNumber, int pageSize);
        Task<ApiResponse<TopUpTransactionReadDto>> GetById(int id);

        //Task<ApiResponse<BalanceReadDto>> GetUserBalance(int userId);
        //Task<ApiResponse<bool>> DebitUserBalance(int userId, decimal amount);
        Task<bool> CanTopUpAmount(int userId, decimal amount);
        Task<bool> CanTopUpBeneficiary(int userId);
        Task<bool> CanTopUpTotal(int userId, decimal amount);
        Task<IEnumerable<TopUpOptionDTO>> GetAvailableTopUpOptions();
        Task<ApiResponse<bool>> DebitUserTopUpTransaction(int userId, decimal amount);
    }
}
