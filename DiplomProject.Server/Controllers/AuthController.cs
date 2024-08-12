using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;


namespace DiplomProject.Server.Controllers
{
	[ApiController]
	[Route("api/[controller]")]
	public class AuthController : ControllerBase
	{
		private readonly IAuthenticationService _authenticationService;

		public AuthController(IAuthenticationService authenticationService)
		{
			_authenticationService = authenticationService;
		}

		[HttpPost("login")]
		public async Task<ActionResult> Login([FromBody] TelegramUserDto user, CancellationToken ct)
		{
			try
			{
				var token = await _authenticationService.AuthenticateUserAsync(user, ct);

				if (String.IsNullOrWhiteSpace(token))
					return Unauthorized();

				return Ok(new { token = token });
			}
			catch (Exception)
			{
				return Unauthorized();
			}
		}
	}
}
