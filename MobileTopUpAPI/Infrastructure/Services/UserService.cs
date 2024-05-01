using Application.Common.Interfaces.IRepositories;
using Application.Common.Models;
using Application.Common.Models.Dtos;
using AutoMapper;
using Azure;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using MobileTopUpAPI.Application.Common.Interfaces.IServices;
using MobileTopUpAPI.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Services
{
    public class UserService : IUserService
    {
        private readonly IGenericRepository<User, int> _UserRepository;
        private readonly IMapper _mapper;

        public UserService(IGenericRepository<User,int> UserRepository,
            IMapper mapper)
        {
            _UserRepository = UserRepository;
            _mapper = mapper;
        }
        /// <summary>
        /// Create new user Account
        /// </summary>
        /// <param name="UserCreateDto"></param>
        /// <returns></returns>
        public async Task<ApiResponse<ApiResponse<UserReadDto>>> Create(UserCreateDto UserCreateDto)
        {
            var apiResponse = new ApiResponse<ApiResponse<UserReadDto>>();
            try
            {
                // Map the UserCreateDto to an User entity
                var User = _mapper.Map<User>(UserCreateDto);

                // Insert the new User into the repository
                var insertedUser = await _UserRepository.InsertAsync(User);

                // Map the inserted User to an UserReadDto
                var insertedUserDto = _mapper.Map<UserReadDto>(insertedUser);

                // Prepare the ApiResponse
                apiResponse.Success = true;
                apiResponse.StatusCode = StatusCodes.Status200OK;
                apiResponse.Message = "User created successfully";
                apiResponse.Data = new ApiResponse<UserReadDto> { Data = insertedUserDto };

                return apiResponse;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"Failed to create User: {ex.Message}";
                apiResponse.Data = null;

                return apiResponse;
            }
        }


        public async Task<ApiResponse<UserReadDto>> CreateOrUpdate(UserCreateDto UserCreateDto)
        {
            var apiResponse = new ApiResponse<UserReadDto>();
            try
            {
                // Check if the User already exists
                var existingUser = await _UserRepository.GetByIdAsync(UserCreateDto.Id);
                if (existingUser != null)
                {
                    // User exists, update it
                    var updatedUser = _mapper.Map(UserCreateDto, existingUser);
                    await _UserRepository.UpdateAsync(updatedUser);

                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status200OK;
                    apiResponse.Message = $"User with ID {UserCreateDto.Id} has been updated";
                    apiResponse.Data = _mapper.Map<UserReadDto>(updatedUser);
                }
                else
                {
                    // User doesn't exist, create it
                    var newUser = _mapper.Map<User>(UserCreateDto);
                    var insertedUser = await _UserRepository.InsertAsync(newUser);

                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status201Created;
                    apiResponse.Message = "New User has been created";
                    apiResponse.Data = _mapper.Map<UserReadDto>(insertedUser);
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"Failed to create or update User: {ex.Message}";
            }
            return apiResponse;
        }


        public async Task<ApiResponse<bool>> Delete(UserCreateDto UserCreateDto)
        {
            var apiResponse = new ApiResponse<bool>();
            try
            {
                // Check if the User exists before attempting to delete it
                var UserExists = await _UserRepository.Queryable().AnyAsync(x => x.Id == UserCreateDto.Id);
                if (UserExists)
                {
                    // Delete the User
                    await _UserRepository.DeleteAsync(UserCreateDto.Id);

                    // Update the ApiResponse
                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status200OK;
                    apiResponse.Message = $"{UserCreateDto.Id} has been successfully deleted";
                    apiResponse.Data = true;
                }
                else
                {
                    // The User does not exist
                    apiResponse.Success = false;
                    apiResponse.StatusCode = StatusCodes.Status404NotFound;
                    apiResponse.Message = $"{UserCreateDto.Id} was not found";
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"An error occurred while deleting {UserCreateDto.Id}: {ex.Message}";
            }
            return apiResponse;
        }

        public async Task<ApiResponse<bool>> Delete(int id)
        {
            var apiResponse = new ApiResponse<bool>();
            try
            {
                // Check if the User exists before attempting to delete it
                var UserExists = await _UserRepository.Queryable().AnyAsync(x => x.Id == id);
                if (UserExists)
                {
                    // Delete the User
                    await _UserRepository.DeleteAsync(id);

                    // Update the ApiResponse
                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status200OK;
                    apiResponse.Message = $"{id} has been successfully deleted";
                    apiResponse.Data = true;
                }
                else
                {
                    // The User does not exist
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

        public async Task<PaginatedList<UserReadDto>> GetAll(int pageNumber, int pageSize)
        {
            try
            {
                // Retrieve all Users from the repository, but only for the specified page
                var UsersQuery = _UserRepository
                    .Queryable()
                    .AsNoTracking();
                var paginatedUsers = await PaginatedList<User>.CreateAsync(UsersQuery, pageNumber, pageSize);

                // Map the Users to UserReadDto
                var UserDtos = _mapper.Map<List<UserReadDto>>(paginatedUsers.Items);

                // Create a PaginatedList<UserReadDto> with the mapped Users
                var paginatedUserDtos = new PaginatedList<UserReadDto>(
                    UserDtos,
                    paginatedUsers.TotalCount,
                    paginatedUsers.PageNumber,
                    paginatedUsers.TotalPages
                );

                return paginatedUserDtos;
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're rethrowing the exception here
                throw new Exception("Failed to retrieve paginated Users", ex);
            }
        }


        public async Task<ApiResponse<UserReadDto>> GetById(int id)
        {
            var apiResponse = new ApiResponse<UserReadDto>();
            try
            {
                // Retrieve the User with the specified ID from the repository
                var User = await _UserRepository.GetByIdAsync(id);
                if (User != null)
                {
                    // Map the User to UserReadDto
                    var UserDto = _mapper.Map<UserReadDto>(User);

                    // Prepare the ApiResponse
                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status200OK;
                    apiResponse.Message = $"User with ID {id} retrieved successfully";
                    apiResponse.Data = UserDto;
                }
                else
                {
                    // User with the specified ID was not found
                    apiResponse.Success = false;
                    apiResponse.StatusCode = StatusCodes.Status404NotFound;
                    apiResponse.Message = $"User with ID {id} not found";
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"Failed to retrieve User with ID {id}: {ex.Message}";
            }
            return apiResponse;
        }

        public async Task<bool> IsUserVerifiedAsync(int userId)
        {
            try
            {
                // Retrieve the User with the specified ID from the repository
                var user = await _UserRepository.Queryable().AnyAsync(x => x.Id == userId && x.IsVerified);
                return user;

            }
            catch(Exception ex)
            {
                return false;
            }
        }

        public async Task<ApiResponse<UserReadDto>> Update(UserCreateDto UserCreateDto, CancellationToken cancellationToken)
        {
            var apiResponse = new ApiResponse<UserReadDto>();
            try
            {
                // Retrieve the User with the specified ID from the repository
                var existingUser = await _UserRepository.GetByIdAsync(UserCreateDto.Id);
                if (existingUser != null)
                {
                    // Map the properties from the UserCreateDto to the existing User entity
                    _mapper.Map(UserCreateDto, existingUser);

                    // Update the existing User in the repository
                    await _UserRepository.UpdateAsync(existingUser);

                    // Map the updated User to UserReadDto
                    var updatedUserDto = _mapper.Map<UserReadDto>(existingUser);

                    // Prepare the ApiResponse
                    apiResponse.Success = true;
                    apiResponse.StatusCode = StatusCodes.Status200OK;
                    apiResponse.Message = $"User with ID {UserCreateDto.Id} updated successfully";
                    apiResponse.Data = updatedUserDto;
                }
                else
                {
                    // User with the specified ID was not found
                    apiResponse.Success = false;
                    apiResponse.StatusCode = StatusCodes.Status404NotFound;
                    apiResponse.Message = $"User with ID {UserCreateDto.Id} not found";
                }
            }
            catch (Exception ex)
            {
                // Log the exception or handle it appropriately
                // For simplicity, we're setting a generic error message here
                apiResponse.Success = false;
                apiResponse.StatusCode = StatusCodes.Status500InternalServerError;
                apiResponse.Message = $"Failed to update User with ID {UserCreateDto.Id}: {ex.Message}";
            }
            return apiResponse;
        }

    }
}
