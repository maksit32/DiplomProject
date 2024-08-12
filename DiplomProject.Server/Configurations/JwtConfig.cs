using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiplomProject.Server.Configurations
{
	public class JwtConfig
	{
		[Required] public string SigningKey { get; set; } = null!;

		public TimeSpan LifeTime { get; set; }

		[Required] public string Audience { get; set; } = null!;

		[Required] public string Issuer { get; set; } = null!;

		public byte[] SigningKeyBytes => Encoding.UTF8.GetBytes(SigningKey);
	}
}
