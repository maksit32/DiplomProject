using DiplomProject.Server.Configurations;
using Domain.Entities;
using Domain.Services.Interfaces;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Telegram.Bot.Types;


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
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = CreateClaimsIdentity(user),
				Expires = DateTime.UtcNow.Add(_jwtConfig.LifeTime),
				Audience = _jwtConfig.Audience,
				Issuer = _jwtConfig.Issuer,
				SigningCredentials = new SigningCredentials(
					new SymmetricSecurityKey(_jwtConfig.SigningKeyBytes),
					SecurityAlgorithms.HmacSha256Signature
				)
			};
			var tokenHandler = new JwtSecurityTokenHandler();
			var securityToken = tokenHandler.CreateToken(tokenDescriptor);
			return tokenHandler.WriteToken(securityToken);
		}
		private ClaimsIdentity CreateClaimsIdentity(TelegramUser user)
		{
			var claimsIdentity = new ClaimsIdentity(new[]
			{
			new Claim(ClaimTypes.NameIdentifier, user.Id.ToString())
			});

			return claimsIdentity;
		}
	}
}
