using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.Extensions.Configuration;

public class JwtTokenService
{
    private readonly IConfiguration _config;

    public JwtTokenService(IConfiguration config)
    {
        _config = config;
    }
    //public string GenerateToken(string userId, string username, string role)
    //{
    //    var claims = new[]
    //    {
    //    new Claim(ClaimTypes.NameIdentifier, userId),   // User ID
    //    new Claim(ClaimTypes.Name, username),          // Username
    //    new Claim(ClaimTypes.Role, role)               // Role
    //};
    //    var jwt = _config.GetSection("Jwt");
    //    var key = new SymmetricSecurityKey(
    //            Encoding.UTF8.GetBytes(jwt["Key"])
    //        );
    //    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    //    var token = new JwtSecurityToken(
    //        issuer: "YourApp",
    //        audience: "YourApp",
    //        claims: claims,
    //        expires: DateTime.Now.AddHours(1),
    //        signingCredentials: creds
    //    );

    //    return new JwtSecurityTokenHandler().WriteToken(token);
    //}
    public string GenerateToken(string userId, string userName, string role)
    {
        var jwt = _config.GetSection("Jwt");

        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, userId),
            new Claim(ClaimTypes.Name, userName),
            new Claim(ClaimTypes.Role, role),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
        };

        var key = new SymmetricSecurityKey(
            Encoding.UTF8.GetBytes(jwt["Key"])
        );

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: jwt["Issuer"],
            audience: jwt["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(
                Convert.ToDouble(jwt["ExpiryMinutes"])
            ),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}
