using Jewellery.Domain.Entities;
using MediatR;

namespace Jewellery.Application.Auth
{
    public class LogoutCommand : IRequest<ResponseModel>
    {
        public string UserId { get; set; }
    }
    public class LogoutCommandHandler
        : IRequestHandler<LogoutCommand, ResponseModel>
    {
        public async Task<ResponseModel> Handle(
            LogoutCommand request,
            CancellationToken cancellationToken)
        {
            return new ResponseModel
            {
                Code = 1,
                Message = "Logout successful."
            };
        }
    }
}
