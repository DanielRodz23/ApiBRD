using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace ApiBRD.Helpers
{
    public class JwtTokerGenerator(IConfiguration configuration)
    {
        private readonly IConfiguration configuration = configuration;

        public string GetToken(string username, string role, int id, List<Claim> claims)
        {
            JwtSecurityTokenHandler handler = new();
            var issuer = configuration.GetSection("Jwt").GetValue<string>("Issuer");
            var audience = configuration.GetSection("Jwt").GetValue<string>("Audience");
            var secret = configuration.GetSection("Jwt").GetValue<string>("Secret");

            List<Claim> basicas =
            [
                new Claim(ClaimTypes.Name, username),
                new Claim(ClaimTypes.Role, role),
                new Claim("IdUsuario", id.ToString()),
                new Claim(JwtRegisteredClaimNames.Iss, issuer),
                new Claim(JwtRegisteredClaimNames.Aud, audience),
                .. claims,
            ];

            JwtSecurityToken jwt = new(
                issuer,
                audience,
                basicas,
                DateTime.Now,
                DateTime.Now.AddMinutes(50),
                new SigningCredentials
                (new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret ?? "")),
                SecurityAlgorithms.HmacSha256)
            );
            return handler.WriteToken(jwt);
        }
    }
}
