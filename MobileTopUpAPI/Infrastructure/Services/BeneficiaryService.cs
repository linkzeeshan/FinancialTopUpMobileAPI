using Application.Common.Interfaces.IRepositories;
using Application.Common.Models;
using Application.Common.Models.Dtos;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using MobileTopUpAPI.Application.Common.Interfaces.IServices;
using MobileTopUpAPI.Application.Common.Models.Dtos;
using MobileTopUpAPI.Domain;
using MobileTopUpAPI.Domain.Entities;

namespace MobileTopUpAPI.Infrastructure.Services
{
    public class BeneficiaryService : IBeneficiaryService
    {

        private readonly IGenericRepository<Beneficiary, int> _beneficiaryRepository;
        private readonly IMapper _mapper;

        public BeneficiaryService(IGenericRepository<Beneficiary, int> BeneficiaryRepository,
            IMapper mapper)
        {
            _beneficiaryRepository = BeneficiaryRepository;
            _mapper = mapper;
        }

        public async Task<ApiResponse<BeneficiaryReadDto>> AddBeneficiary(BeneficiaryCreateDto beneficiaryCreateDto)
        {
            var apiResponse = new ApiResponse<BeneficiaryReadDto>();
            try
            {
                bool beneficiaryExceedLimit = HasReachedBeneficiaryLimit(beneficiaryCreateDto.UserId);

                if (HasReachedBeneficiaryLimit(beneficiaryCreateDto.UserId) is false)
                {
                    var count = _beneficiaryRepository.Queryable()
                        .Where(x => x.UserId == 1).Count();
                    // Check if the Beneficiary already exists
                    var existingBeneficiary = await _beneficiaryRepository.Queryable().FirstOrDefaultAsync(x => x.Nickname.ToLower() == beneficiaryCreateDto.Nickname.ToLower());
                    if (existingBeneficiary != null)
                    {
                        apiResponse.Success = false;
                        apiResponse.StatusCode = StatusCodes.Status208AlreadyReported;
                        apiResponse.Message = $"Beneficiary nickname {beneficiaryCreateDto.Nickname} with User Id {beneficiaryCreateDto.UserId} has been alrerady exist";
                        apiResponse.Data = null;
                    }
                    else
                    {
                        // Beneficiary doesn't exist, create it
                        var newBeneficiary = _mapper.Map<Beneficiary>(beneficiaryCreateDto);
                        var insertedBeneficiary = await _beneficiaryRepository.InsertAsync(newBeneficiary);

                        apiResponse.Success = true;
                        apiResponse.StatusCode = StatusCodes.Status201Created;
                        apiResponse.Message = "New Beneficiary has been created";
                        apiResponse.Data = _mapper.Map<BeneficiaryReadDto>(insertedBeneficiary);
                    }
                }
                else
                {
                    apiResponse.Success = false;
                    apiResponse.StatusCode = StatusCodes.Status510NotExtended;
                    apiResponse.Message = "User can add a maximum of 5 active top-up beneficiaries";
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"Failed to create or update Beneficiary: {ex.Message}";
            }
            return apiResponse;
        }
        public async Task<ApiResponse<BeneficiaryReadDto>> UpdateBeneficiary(BeneficiaryCreateDto beneficiaryCreateDto)
        {
            var apiResponse = new ApiResponse<BeneficiaryReadDto>();
            try
            {
                    // Check if the Beneficiary already exists
                var existingBeneficiary = await _beneficiaryRepository.Queryable().FirstOrDefaultAsync(x => x.UserId == beneficiaryCreateDto.UserId);
                if (existingBeneficiary != null)
                {
                    // Beneficiary exists, update it
                    var updatedBeneficiary = _mapper.Map(beneficiaryCreateDto, existingBeneficiary);
                    updatedBeneficiary.LastModified = DateTime.UtcNow;

                    await _beneficiaryRepository.UpdateAsync(updatedBeneficiary);

                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status200OK;
                    apiResponse.Message = $"Beneficiary with User Id {beneficiaryCreateDto.UserId} has been updated";
                    apiResponse.Data = _mapper.Map<BeneficiaryReadDto>(updatedBeneficiary);
                }
                else
                {
                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status201Created;
                    apiResponse.Message = "User can add a maximum of 5 active top-up beneficiaries";
                }

            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"Failed to create or update Beneficiary: {ex.Message}";
            }
            return apiResponse;
        }
        public async Task<ApiResponse<bool>> Unactivated(int userId)
        {
            var apiResponse = new ApiResponse<bool>();
            try
            {
                // Check if the Beneficiary exists before attempting to delete it
                var beneficiaryExists = await _beneficiaryRepository.Queryable().AnyAsync(x => x.UserId == userId);
                if (beneficiaryExists)
                {
                    var beneficiary = await _beneficiaryRepository.Queryable().FirstOrDefaultAsync(x => x.Id == userId);
                     beneficiary.IsActive = false;

                    // un activated  the Beneficiary
                    await _beneficiaryRepository.UpdateAsync(beneficiary);

                    // Update the ApiResponse
                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status200OK;
                    apiResponse.Message = $"Your beneficiary {beneficiary.Nickname} has been successfully un actived";
                    apiResponse.Data = true;
                }
                else
                {
                    // The Beneficiary does not exist
                    apiResponse.Success = false;
                    apiResponse.StatusCode = StatusCodes.Status404NotFound;
                    apiResponse.Message = $"beneficiary account against user Id {userId} was not found";
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"An error occurred while deleting {userId}: {ex.Message}";
            }
            return apiResponse;
        }

        public async Task<PaginatedList<BeneficiaryReadDto>> GetAll(int pageNumber, int pageSize)
        {
            try
            {
                // Retrieve all Beneficiarys from the repository, but only for the specified page
                var BeneficiarysQuery = _beneficiaryRepository
                    .Queryable()
                    .AsNoTracking();
                var paginatedBeneficiarys = await PaginatedList<Beneficiary>.CreateAsync(BeneficiarysQuery, pageNumber, pageSize);

                // Map the Beneficiarys to BeneficiaryReadDto
                var BeneficiaryDtos = _mapper.Map<List<BeneficiaryReadDto>>(paginatedBeneficiarys.Items);

                // Create a PaginatedList<BeneficiaryReadDto> with the mapped Beneficiarys
                var paginatedBeneficiaryDtos = new PaginatedList<BeneficiaryReadDto>(
                    BeneficiaryDtos,
                    paginatedBeneficiarys.TotalCount,
                    paginatedBeneficiarys.PageNumber,
                    paginatedBeneficiarys.TotalPages
                );

                return paginatedBeneficiaryDtos;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're rethrowing the exception here
                throw new Exception("Failed to retrieve paginated Beneficiarys", ex);
            }
        }

        public async Task<ApiResponse<BeneficiaryReadDto>> GetActiveBeneficiariesByUserId(int userId)
        {
            var apiResponse = new ApiResponse<BeneficiaryReadDto>();
            try
            {
                // Retrieve the Beneficiary with the specified ID from the repository
                var Beneficiary =  _beneficiaryRepository.Queryable()
                    .Where(x => x.UserId == userId && x.IsActive == true)
                    .ToList();

                if (Beneficiary != null)
                {
                    // Map the Beneficiary to BeneficiaryReadDto
                    var BeneficiaryDto = _mapper.Map<BeneficiaryReadDto>(Beneficiary);

                    // Prepare the ApiResponse
                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status200OK;
                    apiResponse.Message = $"Beneficiary with ID {userId} retrieved successfully";
                    apiResponse.Data = BeneficiaryDto;
                }
                else
                {
                    // Beneficiary with the specified ID was not found
                    apiResponse.Success = false;
                    apiResponse.StatusCode = StatusCodes.Status404NotFound;
                    apiResponse.Message = $"Beneficiary with ID {userId} not found";
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"Failed to retrieve Beneficiary with user ID {userId}: {ex.Message}";
            }
            return apiResponse;
        }

        public async Task<int> GetActiveBeneficiariesCountByUserId(int userId)
        {
            try
            {
                // Retrieve the Beneficiary with the specified ID from the repository
                var beneficiary = _beneficiaryRepository.Queryable()
                    .Where(x => x.UserId == userId && x.IsActive == true);

                if (beneficiary != null)
                {
                  return beneficiary.Count();
                }
                else
                {
                    // Beneficiary with the specified ID was not found
                    return 0;
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                return 0;
            }
        }

        #region Private Methhod
        private bool HasReachedBeneficiaryLimit(int userId)
        {
            var beneficiarycount = _beneficiaryRepository.Queryable()
                         .Where(x => x.UserId == userId).Count();
            //Max beneficiary limit is 5
            return beneficiarycount >= Constants.MAX_BENEFICIARY_LIMIT;
        }
        #endregion
    }
}
