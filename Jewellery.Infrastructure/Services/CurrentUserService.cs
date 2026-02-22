using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Jewellery.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace Jewellery.Infrastructure.Services
{
    public class CurrentUserService : ICurrentUserService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        public CurrentUserService(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public string UserId =>
    _httpContextAccessor.HttpContext?.User?
    .FindFirst(ClaimTypes.NameIdentifier)?.Value
    ??
    _httpContextAccessor.HttpContext?.User?
    .FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

        public string UserName =>
            _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Name)?.Value;

        public string Role =>
            _httpContextAccessor.HttpContext?.User?
            .FindFirst(ClaimTypes.Role)?.Value;
    }
}