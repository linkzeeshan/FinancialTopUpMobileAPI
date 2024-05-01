using Application.Common.Interfaces.IRepositories;
using Application.Common.Models;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MobileTopUpAPI.Application.Common.Interfaces.IServices;
using MobileTopUpAPI.Application.Common.Models.Dtos;
using MobileTopUpAPI.Domain.Entities;

namespace MobileTopUpAPI.Infrastructure.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly IGenericRepository<Balance, int> _balanceRepository;
        private readonly IMapper _mapper;

        public BalanceService(IGenericRepository<Balance, int> BalanceRepository,
            IMapper mapper)
        {
            _balanceRepository = BalanceRepository;
            _mapper = mapper;
        }
        public async Task<ApiResponse<ApiResponse<BalanceReadDto>>> Add(BalanceCreateDto balanceCreateDto)
        {
            var apiResponse = new ApiResponse<ApiResponse<BalanceReadDto>>();
            try
            {
                // Map the BalanceCreateDto to an Balance entity
                var Balance = _mapper.Map<Balance>(balanceCreateDto);

                // Insert the new Balance into the repository
                var insertedBalance = await _balanceRepository.InsertAsync(Balance);

                // Map the inserted Balance to an BalanceReadDto
                var insertedBalanceDto = _mapper.Map<BalanceReadDto>(insertedBalance);

                // Prepare the ApiResponse
                apiResponse.Success = true;
                apiResponse.StatusCode = StatusCodes.Status200OK;
                apiResponse.Message = "Balance created successfully";
                apiResponse.Data = new ApiResponse<BalanceReadDto> { Data = insertedBalanceDto };

                return apiResponse;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"Failed to create Balance: {ex.Message}";
                apiResponse.Data = null;

                return apiResponse;
            }
        }

        public async Task<ApiResponse<BalanceReadDto>> CreditOrUpdate(BalanceCreateDto balanceCreateDto)
        {
            var apiResponse = new ApiResponse<BalanceReadDto>();
            try
            {
                // Check if the Balance already exists
                var existingBalance = await _balanceRepository.Queryable()
                    .Include(b => b.User)
                    .FirstOrDefaultAsync(x => x.UserId == balanceCreateDto.UserId);
                if (existingBalance != null)
                {
                    existingBalance.Amount += balanceCreateDto.Amount;
                    // Balance exists, update it
                    var updatedBalance = _mapper.Map(balanceCreateDto, existingBalance);
                    updatedBalance.LastModified = DateTimeOffset.UtcNow;    
                    await _balanceRepository.UpdateAsync(updatedBalance);

                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status200OK;
                    apiResponse.Message = $"Balance with User {existingBalance.User.UserName} has been updated";
                    apiResponse.Data = _mapper.Map<BalanceReadDto>(updatedBalance);
                }
                else
                {
                    // Balance doesn't exist, create it
                    var newBalance = _mapper.Map<Balance>(balanceCreateDto);
                    var insertedBalance = await _balanceRepository.InsertAsync(newBalance);

                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status201Created;
                    apiResponse.Message = $"New Balance has been created for {balanceCreateDto.UserId}";
                    apiResponse.Data = _mapper.Map<BalanceReadDto>(insertedBalance);
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"Failed to create or update Balance: {ex.Message}";
            }
            return apiResponse;
        }

        public async Task<bool> DebitBalanceByUserIdAsync(int userId, decimal amount)
        {
            try
            {
                // Find the user's balance record in the Balance table
                var balance = await _balanceRepository.Queryable().FirstOrDefaultAsync(b => b.UserId == userId);

                if (balance != null)
                {
                    // Check if the user has sufficient balance to debit
                    if (balance.Amount >= amount)
                    {
                        // Deduct the amount from the user's balance
                        balance.Amount -= amount;

                        await _balanceRepository.UpdateAsync(balance);
                        // Save the changes to the database
                        await _balanceRepository.SaveChangesAsync();

                        // Debit successful
                        return true;
                    }
                    else
                    {
                        // Insufficient balance
                        return false;
                    }
                }
                else
                {
                    // User not found in the Balance table
                    return false;
                }
            }
            catch (Exception ex)
            {
                // Handle any exceptions that may occur during the process
                // You may log the exception, throw it, or handle it according to your application's requirements
                return false;
            }
        }

        public async Task<decimal> GetBalanceByUserIdAsync(int userId)
        {
            try
            {
                var balance = await _balanceRepository.Queryable().FirstOrDefaultAsync(b => b.UserId == userId);
                if (balance is null)
                    return 0;
                else
                    return balance.Amount;

            }
            catch (Exception ex)
            {
                throw new Exception();
            }
        }
        public async Task<ApiResponse<bool>> Delete(int id)
        {
            var apiResponse = new ApiResponse<bool>();
            try
            {
                // Check if the Balance exists before attempting to delete it
                var BalanceExists = await _balanceRepository.Queryable().AnyAsync(x => x.Id == id);
                if (BalanceExists)
                {
                    // Delete the Balance
                    await _balanceRepository.DeleteAsync(id);

                    // Update the ApiResponse
                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status200OK;
                    apiResponse.Message = $"{id} has been successfully deleted";
                    apiResponse.Data = true;
                }
                else
                {
                    // The Balance does not exist
                    apiResponse.Success = false;
                    apiResponse.StatusCode = StatusCodes.Status404NotFound;
                    apiResponse.Message = $"{id} was not found";
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"An error occurred while deleting {id}: {ex.Message}";
            }
            return apiResponse;
        }

        public async Task<PaginatedList<BalanceReadDto>> GetAll(int pageNumber, int pageSize)
        {
            try
            {
                // Retrieve all Balances from the repository, but only for the specified page
                var BalancesQuery = _balanceRepository
                    .Queryable()
                    .AsNoTracking();
                var paginatedBalances = await PaginatedList<Balance>.CreateAsync(BalancesQuery, pageNumber, pageSize);

                // Map the Balances to BalanceReadDto
                var BalanceDtos = _mapper.Map<List<BalanceReadDto>>(paginatedBalances.Items);

                // Create a PaginatedList<BalanceReadDto> with the mapped Balances
                var paginatedBalanceDtos = new PaginatedList<BalanceReadDto>(
                    BalanceDtos,
                    paginatedBalances.TotalCount,
                    paginatedBalances.PageNumber,
                    paginatedBalances.TotalPages
                );

                return paginatedBalanceDtos;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're rethrowing the exception here
                throw new Exception("Failed to retrieve paginated Balances", ex);
            }
        }

        public async Task<ApiResponse<BalanceReadDto>> GetById(int id)
        {
            var apiResponse = new ApiResponse<BalanceReadDto>();
            try
            {
                // Retrieve the Balance with the specified ID from the repository
                var Balance = await _balanceRepository.GetByIdAsync(id);
                if (Balance != null)
                {
                    // Map the Balance to BalanceReadDto
                    var BalanceDto = _mapper.Map<BalanceReadDto>(Balance);

                    // Prepare the ApiResponse
                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status200OK;
                    apiResponse.Message = $"Balance with ID {id} retrieved successfully";
                    apiResponse.Data = BalanceDto;
                }
                else
                {
                    // Balance with the specified ID was not found
                    apiResponse.Success = false;
                    apiResponse.StatusCode = StatusCodes.Status404NotFound;
                    apiResponse.Message = $"Balance with ID {id} not found";
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"Failed to retrieve Balance with ID {id}: {ex.Message}";
            }
            return apiResponse;
        }
    }
}
