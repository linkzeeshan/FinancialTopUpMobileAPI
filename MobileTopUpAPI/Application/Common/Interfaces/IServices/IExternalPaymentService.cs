namespace MobileTopUpAPI.Application.Common.Interfaces.IServices
{
    public interface IExternalPaymentService
    {
        Task<decimal> GetBalanceAsync(int userId);
        Task<bool> DebitBalanceAsync(int userId, decimal amount);
    }
}
