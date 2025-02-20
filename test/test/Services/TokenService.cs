using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using test.Models.Entity;
using test.Models;

namespace test.Services
{
    public class TokenService 
    {
        private readonly IConfiguration _configuration;

        public TokenService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(User user)
        {   
            var claims = new List<Claim>
            {   
                new Claim("id", user.Id.ToString()),
                new Claim("email", user.Email ?? ""),
                new Claim("username", user.Username ?? ""),
                new Claim("roles", string.Join(",", user.UserRoles?.Select(ur => ur.Role?.Name) ?? Array.Empty<string>()))
            };
            
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var securityTokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.Now.AddHours(1),
                SigningCredentials = creds,
                Issuer = _configuration["Jwt:Issuer"],
                Audience = _configuration["Jwt:Audience"]
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.CreateToken(securityTokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public (string name, string email) GetUserInfoFromToken(string token)
        {
            if (string.IsNullOrEmpty(token))
                return (null, null);

            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);

            var name = jwtToken.Claims.FirstOrDefault(c => c.Type == "username")?.Value;
            var email = jwtToken.Claims.FirstOrDefault(c => c.Type == "email")?.Value;

            return (name, email);
        }

        public (UserAccessInfo userInfo, bool isValid) ValidateAndGetUserAccess(string token)
        {
            if (string.IsNullOrEmpty(token))
                return (null, false);

            var (name, email) = GetUserInfoFromToken(token);

            if (string.IsNullOrEmpty(name) || string.IsNullOrEmpty(email))
                return (null, false);

            return (new UserAccessInfo
            {
                Name = name,
                Email = email,
                ConnectedTime = DateTime.Now
            }, true);
        }
    }
} 