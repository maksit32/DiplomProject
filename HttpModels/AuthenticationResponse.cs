using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpModels
{
	public class AuthenticationResponse
	{
		[EmailAddress]
		public string Email { get; set; } = null!;

		public AuthenticationResponse(string email)
		{
			Email = email;
		}
	}
}
