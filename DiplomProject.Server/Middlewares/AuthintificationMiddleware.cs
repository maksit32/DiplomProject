
namespace DiplomProject.Server.Middlewares
{
	//must be deleted!
	public class AuthenticationMiddleware
	{
		private readonly RequestDelegate next;
		private readonly ILogger<AuthenticationMiddleware> _logger;
		public AuthenticationMiddleware(RequestDelegate next, ILogger<AuthenticationMiddleware> logger)
		{
			this.next = next;
			_logger = logger;
		}
		public async Task InvokeAsync(HttpContext context)
		{
			_logger.LogInformation("________________________________________________________");
			_logger.LogInformation("{@request}", context.Request.Headers);
			_logger.LogInformation("________________________________________________________");
			await next.Invoke(context);
		}
	}
}
