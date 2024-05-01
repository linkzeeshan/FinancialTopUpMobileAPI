using Application.Common.Models;
using Application.Common.Models.Dtos;

namespace MobileTopUpAPI.Application.Common.Interfaces.IServices
{
    public interface IUserService
    {
        Task<ApiResponse<ApiResponse<UserReadDto>>> Create(UserCreateDto UserCreateDto);
        Task<ApiResponse<UserReadDto>> Update(UserCreateDto UserCreateDto, CancellationToken cancellationToken);
        Task<ApiResponse<bool>> Delete(UserCreateDto UserCreateDto);
        Task<ApiResponse<bool>> Delete(int id);
        Task<ApiResponse<UserReadDto>> CreateOrUpdate(UserCreateDto UserCreateDto);
        Task<PaginatedList<UserReadDto>> GetAll(int pageNumber, int pageSize);
        Task<ApiResponse<UserReadDto>> GetById(int id);
        Task<bool> IsUserVerifiedAsync(int userId);
    }
}
