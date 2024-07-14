using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpModels
{
	public class RegistrationResponse
	{
		[StringLength(100, MinimumLength = 2), Required] public string Name { get; set; } = null!;
		[StringLength(20, MinimumLength = 7), Required] public string Login { get; set; } = null!;

		[EmailAddress, Required] public string Email { get; set; } = null!;

		public RegistrationResponse(string name, string email, string login)
		{
			Name = name;
			Email = email;
			Login = login;
		}
	}
}
