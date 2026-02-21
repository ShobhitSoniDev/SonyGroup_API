
using Azure.Core;

namespace Jewellery.Application.Auth.Interfaces
{
    public interface ISignUpRepository
    {
        Task<dynamic> SignUpReturnAsync(string UserName, string Email, string Password, string MobileNo, int Type);

    }
}