using Application.Common.Interfaces.IRepositories;
using Application.Common.Models;
using AutoMapper;
using Azure.Core;
using Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using MobileTopUpAPI.Application.Common.Interfaces.IServices;
using MobileTopUpAPI.Application.Common.Models.Dtos;
using MobileTopUpAPI.Domain;
using MobileTopUpAPI.Domain.Entities;
using System.Reflection.Metadata;

namespace MobileTopUpAPI.Infrastructure.Services
{
    public class TopUpTransactionService : ITopUpTransactionService
    {
        private readonly IGenericRepository<TopUpTransaction, int> _topUpTransactionRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IBeneficiaryService _beneficiaryService;
        private readonly IBalanceService _balanceService;

        public TopUpTransactionService(IGenericRepository<TopUpTransaction, int> topUpTransactionRepository,
           IUserService userService,
           IBeneficiaryService beneficiaryService,
           IBalanceService balanceService,
           IMapper mapper)
        {
            _topUpTransactionRepository = topUpTransactionRepository;
            _beneficiaryService = beneficiaryService;
            _userService = userService;
            _balanceService = balanceService;
            _mapper = mapper;
        }
        /// <summary>
        /// user can top up the specified amount based on verification status
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public async Task<bool> CanTopUpAmount(int userId, decimal amount)
        {
            // Implement logic to check if user can top up the specified amount based on verification status
            bool isVerified = await _userService.IsUserVerifiedAsync(userId);
            if (isVerified)
            {
                return amount <= 500; // Max AED 500 per month per beneficiary for verified users
            }
            else
            {
                return amount <= 1000; // Max AED 1000 per month per beneficiary for unverified users
            }
        }
        /// <summary>
        /// Implement logic to check if user can top up a new beneficiary
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task<bool> CanTopUpBeneficiary(int userId)
        {
            // Implement logic to check if user can top up a new beneficiary
            var beneficiaryCount = await _beneficiaryService.GetActiveBeneficiariesCountByUserId(userId);
            return beneficiaryCount <= 5; // Max 5 active beneficiaries per user
        }
        /// <summary>
        /// Implement logic to check if user can top up the total amount for all beneficiaries
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public async Task<bool> CanTopUpTotal(int userId, decimal amount)
        {
            // Implement logic to check if user can top up the total amount for all beneficiaries
            decimal totalTopUpForMonth = await this.GetTotalTopUpForMonth(userId);
            return totalTopUpForMonth + amount <= 3000; // Max AED 3000 per month for all beneficiaries
        }
        /// <summary>
        /// Prepare transaction history
        /// </summary>
        /// <param name="topUpTransactionCreateDto"></param>
        /// <returns></returns>
        public async Task<ApiResponse<TopUpTransactionReadDto>> CreateTopUpTransactionHistory(TopUpTransactionRequest topUpTransactionrequest)
        {
            var apiResponse = new ApiResponse<TopUpTransactionReadDto>();
            try
            {
                var topUpTransactionCreateDto = _mapper.Map<TopUpTransactionCreateDto>(topUpTransactionrequest);

                //transaction charges is adding 1 AED
                topUpTransactionCreateDto.Charge = Constants.TRANSACTION_CHARGE;
                
                //Calculating Total Amount 
                topUpTransactionCreateDto.TotalAmount = this.CalculateTransactionAmount(topUpTransactionCreateDto.Amount);
                // Check if the TopUpTransaction already exists
                var userverified = await _userService.IsUserVerifiedAsync(topUpTransactionCreateDto.UserId);
                if (userverified)
                {
                    // TopUpTransaction doesn't exist, create it
                    var newTopUpTransaction = _mapper.Map<TopUpTransaction>(topUpTransactionCreateDto);
                    newTopUpTransaction.TransactionDate = DateTime.Now;
                    var insertedTopUpTransaction = await _topUpTransactionRepository.InsertAsync(newTopUpTransaction);

                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status201Created;
                    apiResponse.Message = "New TopUpTransaction has been proceed successfully";
                    apiResponse.Data = _mapper.Map<TopUpTransactionReadDto>(insertedTopUpTransaction);
                }
               
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"Failed to create or update TopUpTransaction: {ex.Message}";
            }
            return apiResponse;
        }
        /// <summary>
        /// Debit Amount + Charges from user account
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="amount"></param>
        /// <returns></returns>
        public async Task<ApiResponse<bool>> DebitUserTopUpTransaction(int userId, decimal amount)
        {
            var apiResponse = new ApiResponse<bool>();
            try
            {
                List<TopUpOptionDTO> ammountCheck =(List<TopUpOptionDTO>) await this.GetAvailableTopUpOptions();
                if (ammountCheck.Any(x => x.Amount == amount))
                {
                    amount = CalculateTransactionAmount(amount);

                    if (await _balanceService.DebitBalanceByUserIdAsync(userId, amount))
                    {

                        apiResponse.Success = true;
                        apiResponse.StatusCode = StatusCodes.Status200OK;
                        apiResponse.Message = $"TopUp Transaction successfully";
                        apiResponse.Data = true;
                    }
                    else
                    {
                        apiResponse.Success = false;
                        apiResponse.StatusCode = StatusCodes.Status200OK;
                        apiResponse.Message = $"Transaction failed selected {amount} is exceeded";
                        apiResponse.Data = false;
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = StatusCodes.Status405MethodNotAllowed;
                    apiResponse.Message = $"TopUpTransaction is not equal to TopUp Options";
                    apiResponse.Data = false;
                }
            }catch(Exception ex)
            {
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"Failed to create TopUpTransaction: {ex.Message}";
            }

            return apiResponse;


        }

        public Task<PaginatedList<TopUpTransactionReadDto>> GetAll(int pageNumber, int pageSize)
        {
            throw new NotImplementedException();
        }

        public async Task<IEnumerable<TopUpOptionDTO>> GetAvailableTopUpOptions()
        {
            // Hardcoded list of available top-up options
            var topUpOptions = new List<TopUpOptionDTO>
            {
                new TopUpOptionDTO { Key=1, Amount = 5 },
                new TopUpOptionDTO { Key=2, Amount = 10 },
                new TopUpOptionDTO { Key=3, Amount = 20 },
                new TopUpOptionDTO { Key=4, Amount = 30 },
                new TopUpOptionDTO { Key=5, Amount = 50 },
                new TopUpOptionDTO { Key=6, Amount = 75 },
                new TopUpOptionDTO { Key=7, Amount = 100 }
            };

            return topUpOptions;
        }

        public Task<ApiResponse<TopUpTransactionReadDto>> GetById(int id)
        {
            throw new NotImplementedException();
        }
        #region  private method
        private async Task<decimal> GetTotalTopUpForMonth(int userId)
        {
            // Get the first and last dates of the current month
            var currentDate = DateTime.Now;
            var firstDayOfMonth = new DateTime(currentDate.Year, currentDate.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);

            // Query the database to get the total top-up amount for the current month for the specified user
            decimal totalTopUpAmount = await _topUpTransactionRepository.Queryable()
                .Where(t => t.UserId == userId && t.TransactionDate >= firstDayOfMonth && t.TransactionDate <= lastDayOfMonth)
                .SumAsync(t => t.TotalAmount);

            return totalTopUpAmount;
        }
        public decimal CalculateTransactionAmount(decimal topUpAmount)
        {
            // Apply charge of AED 1 for every top-up transaction
            return topUpAmount + 1;
        }
        #endregion
    }
}
