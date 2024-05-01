using Application.Common.Models.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MobileTopUpAPI.Application.Common.Interfaces.IServices;
using MobileTopUpAPI.Application.Common.Models.Dtos;

namespace MobileTopUpAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BeneficiaryController : ControllerBase
    {
        private readonly IBeneficiaryService _beneficiaryService;
        public BeneficiaryController(IBeneficiaryService beneficiaryService)
        {
            _beneficiaryService = beneficiaryService;
        }

        [HttpPost]
        [Route("AddBeneficiary")]
        public async Task<IActionResult> AddBeneficiary(BeneficiaryCreateDto beneficiaryCreateDto)
        {
            var response = await _beneficiaryService.AddBeneficiary(beneficiaryCreateDto);
            return Ok(response);
        }
        [HttpPut]
        [Route("UpdateBeneficiary")]
        public async Task<IActionResult> UpdateBeneficiary(BeneficiaryCreateDto beneficiaryCreateDto)
        {
            var response = await _beneficiaryService.UpdateBeneficiary(beneficiaryCreateDto);
            return Ok(response);
        }
        [HttpGet]
        [Route("GetActiveBeneficiariesByUserId/{id}")]
        public async Task<IActionResult> GetActiveBeneficiariesByUserId(int id)
        {
            var response = await _beneficiaryService.GetActiveBeneficiariesByUserId(id);
            return Ok(response);
        }

        [HttpGet]
        [Route("GetActiveBeneficiariesCountByUserId/{id}")]
        public async Task<IActionResult> GetActiveBeneficiariesCountByUserId(int id)
        {
            var response = await _beneficiaryService.GetActiveBeneficiariesCountByUserId(id);
            return Ok(response);
        }
        [HttpGet]
        [Route("UnactivatedBeneficiariesByUserId/{id}")]
        public async Task<IActionResult> UnactivatedBeneficiariesByUserId(int id)
        {
            var response = await _beneficiaryService.GetActiveBeneficiariesCountByUserId(id);
            return Ok(response);
        }


    }
}
