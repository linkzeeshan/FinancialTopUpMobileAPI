using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileTopUpAPI.Application.Common.Interfaces.IServices;
using MobileTopUpAPI.Application.Common.Models.Dtos;
using MobileTopUpAPI.Infrastructure.Services;

namespace MobileTopUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExternalPaymentController : ControllerBase
    {
        private readonly IExternalPaymentService _externalPaymentService;
        public ExternalPaymentController(IExternalPaymentService externalPaymentService)
        {
            _externalPaymentService = externalPaymentService;
        }
        [HttpGet]
        [Route("GetBalanceAsyncByUserId/{id}")]
        public async Task<IActionResult> CreditBalace(int id)
        {
            var response = await _externalPaymentService.GetBalanceAsync(id);
            return Ok(response);
        }
        [HttpPost]
        [Route("DebitBalanceAsync")]
        public async Task<IActionResult> DebitBalanceByUserIdAsync(BalanceCreateDto balanceCreateDto)
        {
            var response = await _externalPaymentService.DebitBalanceAsync(balanceCreateDto.UserId, balanceCreateDto.Amount);
            return Ok(response);
        }
    }
}
