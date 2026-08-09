using Jewellery.Domain.Entities;

namespace Jewellery.Application.Transactions.Interfaces
{
    public interface IAuthRepository
    {
        Task<dynamic> LoginReturnAsync(string username, string shopCode);
        Task<dynamic> SignUpReturnAsync(string UserName, string Email, string Password, string MobileNo, int Type, string shopCode,string Role);
        Task<dynamic> GetMenuReturnAsync();
        Task<dynamic> LoginCustomerReturnAsync(string username, string shopCode);
    }
}

