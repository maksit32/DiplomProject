using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;



namespace HttpModels
{
	public class AuthenticationRequest
	{
		[EmailAddress]
		public string Email { get; set; } = null!;
		[Required]
		public string Password { get; set; } = null!;
	}
}
