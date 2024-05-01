using Application.Common.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileTopUpAPI.Application.Common.Interfaces.IServices;

namespace MobileTopUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ApplicationUserController : ControllerBase
    {
        private readonly IUserService _userService;
        public ApplicationUserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Route("GetAsync/{id}")]
        public async Task<IActionResult> GetAsync(int id)
        {
            var response = await _userService.GetById(id);
            return Ok(response);    
        }
        [HttpPost]
        [Route("CreateOrUpdate")]
        public async Task<IActionResult> CreateOrUpdate(UserCreateDto userCreateDto)
        {
            var response = await _userService.CreateOrUpdate(userCreateDto);
            return Ok(response);
        }
        [HttpDelete]
        [Route("DeleteByUserId/{id}")]
        public async Task<IActionResult> DeleteByUserId(int id)
        {
            var response = await _userService.Delete(id);
            return Ok(response);
        }
        [HttpGet]
        [Route("GetAllAsync")]
        public async Task<IActionResult> GetAllAsync(int pageNumber, int pageSize)
        {
            var response = await _userService.GetAll(pageNumber, pageSize);
            return Ok(response);
        }
        [HttpGet]
        [Route("IsUserVerifiedAsync/{id}")]
        public async Task<IActionResult> IsUserVerifiedAsync(int id)
        {
            var response = await _userService.IsUserVerifiedAsync(id);
            return Ok(response);
        }
    }
}
