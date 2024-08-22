using System.ComponentModel.DataAnnotations;
using System.Text;

namespace DiplomProject.Server.Configurations
{
	public class JwtConfig
	{
		[Required] public string SigningKey { get; set; } = null!;

		[Required] public string Audience { get; set; } = null!;

		[Required] public string Issuer { get; set; } = null!;
	}
}
