using Application.Common.Models;
using Application.Common.Models.Dtos;

namespace MobileTopUpAPI.Application.Common.Interfaces.IServices
{
    public interface IBeneficiaryService
    {
        Task<ApiResponse<BeneficiaryReadDto>> AddBeneficiary(BeneficiaryCreateDto BeneficiaryCreateDto);
        Task<ApiResponse<BeneficiaryReadDto>> UpdateBeneficiary(BeneficiaryCreateDto BeneficiaryCreateDto);
        Task<PaginatedList<BeneficiaryReadDto>> GetAll(int pageNumber, int pageSize);
        Task<ApiResponse<BeneficiaryReadDto>> GetActiveBeneficiariesByUserId(int id);
        Task<int> GetActiveBeneficiariesCountByUserId(int userId);
        Task<ApiResponse<bool>> Unactivated(int id);
    }
}
