
namespace Jewellery.Application.Auth.Interfaces
{
    public interface ILoginRepository
    {
        Task<dynamic> LoginReturnAsync(string username);

    }
}