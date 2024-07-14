using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HttpModels
{
	public class RegistrationRequest
	{
		[StringLength(100, MinimumLength = 2), Required]
		public string Name { get; set; } = null!;
		[StringLength(20, MinimumLength = 7), Required]
		public string Login { get; set; } = null!;

		[EmailAddress, Required]
		public string Email { get; set; } = null!;

		[Required]
		public string Password { get; set; } = null!;
		[Required]
		public bool isAdmin { get; set; }
	}
}
