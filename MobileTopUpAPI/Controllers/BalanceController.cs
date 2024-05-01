using Application.Common.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileTopUpAPI.Application.Common.Interfaces.IServices;
using MobileTopUpAPI.Application.Common.Models.Dtos;
using MobileTopUpAPI.Infrastructure.Services;

namespace MobileTopUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BalanceController : ControllerBase
    {
        private readonly IBalanceService _balanceService;
        public BalanceController(IBalanceService balanceService)
        {
            _balanceService = balanceService;
        }
        [HttpPost]
        [Route("CreditBalance")]
        public async Task<IActionResult> CreditBalace(BalanceCreateDto balanceCreateDto)
        {
            var response = await _balanceService.CreditOrUpdate(balanceCreateDto);
            return Ok(response);
        }
        [HttpPost]
        [Route("DebitBalanceAsync")]
        public async Task<IActionResult> DebitBalanceByUserIdAsync(BalanceCreateDto balanceCreateDto)
        {
            var response = await _balanceService.DebitBalanceByUserIdAsync(balanceCreateDto.UserId, balanceCreateDto.Amount);
            return Ok(response);
        }
        [HttpGet]
        [Route("GetBalanceByUserIdAsync/{id}")]
        public async Task<IActionResult> GetBalanceByUserIdAsync(int id)
        {
            var response = await _balanceService.GetBalanceByUserIdAsync(id);
            return Ok(response);
        }

        


    }
}
