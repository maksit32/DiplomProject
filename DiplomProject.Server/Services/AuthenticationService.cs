using Domain.Entities;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;


namespace DiplomProject.Server.Services
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly IPasswordHasherService _passwordHasherService;
		private readonly ITelegramUserRepository _telegramUserRepository;
		private readonly IJwtService _jwtService;

		public AuthenticationService(IPasswordHasherService passwordHasherService, ITelegramUserRepository telegramUserRepository, IJwtService jwtService)
		{
			_passwordHasherService = passwordHasherService;
			_telegramUserRepository = telegramUserRepository;
			_jwtService = jwtService;
		}

		public async Task<string> AuthenticateUserAsync(TelegramUserDto login, CancellationToken ct)
		{
			var user = await _telegramUserRepository.GetTgUserByPhoneAsync(login.PhoneNumber, ct);
			if (user == null || !_passwordHasherService.VerifyPassword(user.HashedPassword, login.Password) || !user.IsAdmin)
				return string.Empty;

			return _jwtService.GenerateJwtToken(login.PhoneNumber);
		}
	}
}
