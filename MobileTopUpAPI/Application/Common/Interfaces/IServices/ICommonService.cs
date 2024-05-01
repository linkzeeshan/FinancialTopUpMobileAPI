namespace MobileTopUpAPI.Application.Common.Interfaces.IServices
{
    public interface ICommonService
    {
        Task<bool> NickNameExist(string name);
    }
}
