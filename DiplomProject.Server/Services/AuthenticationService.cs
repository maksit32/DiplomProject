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
			_passwordHasherService = passwordHasherService ?? throw new ArgumentNullException(nameof(passwordHasherService));
			_telegramUserRepository = telegramUserRepository ?? throw new ArgumentNullException(nameof(telegramUserRepository));
			_jwtService = jwtService ?? throw new ArgumentNullException(nameof(jwtService));
		}

		public async Task<string> AuthenticateUserAsync(TelegramUserDto userData, CancellationToken ct)
		{
			var user = await _telegramUserRepository.GetTgUserByPhoneAsync(userData.PhoneNumber, ct);
			if (user == null || !_passwordHasherService.VerifyPassword(user.HashedPassword, userData.Password) || !user.IsAdmin)
				return string.Empty;

			return _jwtService.GenerateJwtToken(userData.PhoneNumber);
		}
	}
}
