using DiplomProject.Server.Configurations;
using Domain.Entities;
using Domain.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;



namespace DiplomProject.Server.Services
{
	public class JwtService : IJwtService
	{
		private readonly JwtConfig _jwtConfig;

		public JwtService(IOptions<JwtConfig> jwtConfig)
		{
			ArgumentNullException.ThrowIfNull(jwtConfig);
			_jwtConfig = jwtConfig.Value;
		}
		public string GenerateJwtToken(TelegramUser user)
		{
			var issuer = _jwtConfig.Issuer;
			var audience = _jwtConfig.Audience;
			var key = Encoding.ASCII.GetBytes(_jwtConfig.SigningKey);

			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim("Id", Guid.NewGuid().ToString()),
					new Claim(JwtRegisteredClaimNames.Sub, "Data sync request"),
					new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
				}),
				Expires = DateTime.UtcNow.AddMinutes(15),
				Issuer = issuer,
				Audience = audience,
				SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha512Signature)
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var stringToken = tokenHandler.WriteToken(token);
			return stringToken;
		}
	}
}
