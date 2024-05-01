using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileTopUpAPI.Application.Common.Interfaces.IServices;
using MobileTopUpAPI.Application.Common.Models.Dtos;

namespace MobileTopUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopUpController : ControllerBase
    {
        private readonly ITopUpTransactionService _topUpService;

        public TopUpController(ITopUpTransactionService topUpService)
        {
            _topUpService = topUpService;
        }
        [HttpGet]
        [Route("TopUpAvailableOptions")]
        public async Task<IActionResult> TopUpAvailableOptions()
        {
            return Ok(await _topUpService.GetAvailableTopUpOptions());
        }
        [HttpPost]
        [Route("MobileTopUp")]
        public async Task<IActionResult> TopUp(TopUpTransactionRequest request)
        {
            int userId = 1; // For demo purpose, replace with actual user id from authentication

            // Check if the user can top up the specified amount
            bool canTopUpAmount = await _topUpService.CanTopUpAmount(request.UserId, request.Amount);
            if (!canTopUpAmount)
            {
                return BadRequest("User beneficiaries has exceeded top-up limit.");
            }

            // Check if the user can top up a new beneficiary
            bool canTopUpBeneficiary = await _topUpService.CanTopUpBeneficiary(request.UserId);
            if (!canTopUpBeneficiary)
            {
                return BadRequest("User has reached the maximum limit of beneficiaries.");
            }

            // Check if the user can top up the total amount for all beneficiaries
            bool canTopUpTotal = await _topUpService.CanTopUpTotal(request.UserId, request.Amount);
            if (!canTopUpTotal)
            {
                return BadRequest("User has exceeded total top-up limit for all beneficiaries.");
            }
            //Proceed with Debit User TopUp Transaction
            // Implement logic to process the top-up transaction
            var debitUserAmount =  await _topUpService.DebitUserTopUpTransaction(request.UserId, request.Amount);
            if (debitUserAmount.Success)
                await _topUpService.CreateTopUpTransactionHistory(request);

            return Ok(debitUserAmount);


        }
    }
}

