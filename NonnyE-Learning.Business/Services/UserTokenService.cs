using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using NonnyE_Learning.Business.AppSetting;
using NonnyE_Learning.Business.Services.Interfaces;
using NonnyE_Learning.Data.Models;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace NonnyE_Learning.Business.Services
{
	public class UserTokenService : IUserTokenService
	{
		private readonly JwtSettings _jwtSettings;

		public UserTokenService(IOptions<JwtSettings> jwtOptions)
		{
			_jwtSettings = jwtOptions.Value;
		}

		public string GenerateToken(ApplicationUser user, IList<string> roles)
		{
			var claims = new List<Claim>
		{
			new Claim(JwtRegisteredClaimNames.Sub, user.Email ?? ""),
			new Claim(ClaimTypes.NameIdentifier, user.Id),
			new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}")
		};

			// Add roles to claims
			foreach (var role in roles)
			{
				claims.Add(new Claim(ClaimTypes.Role, role));
			}

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_jwtSettings.Key));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				issuer: _jwtSettings.Issuer,
				audience: _jwtSettings.Audience,
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(_jwtSettings.ExpiryMinutes),
				signingCredentials: creds
			);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
	}
}
