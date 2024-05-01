using MobileTopUpAPI.Application.Common.Interfaces.IServices;
using Newtonsoft.Json;

namespace MobileTopUpAPI.Infrastructure.Services
{
    public class ExternalPaymentService : IExternalPaymentService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        public ExternalPaymentService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri(configuration.GetValue<string>("ExternalPayment:URL"));
            _configuration = configuration;
        }
        public async Task<decimal> GetBalanceAsync(int userId)
        {
            // Make a GET request to retrieve user balance from the external service
            HttpResponseMessage response = await _httpClient.GetAsync($"GetBalanceByUserIdAsync/{userId}");

            if (response.IsSuccessStatusCode)
            {
                string jsonResponse = await response.Content.ReadAsStringAsync();
                var balance = JsonConvert.DeserializeObject<decimal>(jsonResponse);
                return balance;
            }
            else
            {
                // Handle error response
                throw new Exception("Error retrieving balance from the external service.");
            }
        }

        public async Task<bool> DebitBalanceAsync(int userId, decimal amount)
        {
            // Make a POST request to debit user's balance in the external service
            var requestContent = new StringContent(JsonConvert.SerializeObject(new { UserId = userId, Amount = amount }));
            HttpResponseMessage response = await _httpClient.PostAsync("/DebitBalanceAsync", requestContent);

            if (response.IsSuccessStatusCode)
            {
                return true;
            }
            else
            {
                // Handle error response
                throw new Exception("Error debiting balance in the external service.");
            }
        }
    }
}
