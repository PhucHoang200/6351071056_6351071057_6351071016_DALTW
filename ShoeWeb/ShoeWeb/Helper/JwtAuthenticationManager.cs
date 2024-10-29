using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;
using ShoeWeb.Models;

namespace ShoeWeb.Helper
{
    public class JwtAuthenticationManager
    {
        private readonly string _key;
        public JwtAuthenticationManager(string key)
        {
            _key = key;
        }

        public string GenerateToken(User user, Role role, int expireMinutes = 30)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var tokenKey = Encoding.ASCII.GetBytes(_key); // Mã hóa _key
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new Claim[]
                {
                    new Claim(ClaimTypes.Name, user.userName), 
                    new Claim(ClaimTypes.Role, role.NameRole) 
                }),
                Expires = DateTime.UtcNow.AddMinutes(expireMinutes), // Thời hạn token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature) // Phương thức mã hóa
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token); // Trả về token dưới dạng chuỗi
        }

        public ClaimsPrincipal ValidateToken(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_key);

            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var claims = jwtToken.Claims;
            return new ClaimsPrincipal(new ClaimsIdentity(claims));
        }
    }
}