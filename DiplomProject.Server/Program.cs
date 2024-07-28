using DiplomProject.Server.DbContexts;
using DiplomProject.Server.Repositories;
using DiplomProject.Server.Services;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Serilog;
using Serilog.Core;
using Telegram.Bot;




namespace API
{
	public class Program
	{
		public static async Task Main(string[] args)
		{
			Log.Logger = new LoggerConfiguration()
				.WriteTo.Console()
				.CreateLogger();
			Log.Information("Server is up");
			try
			{
				var builder = WebApplication.CreateBuilder(args);
				builder.Host.UseSerilog((hbc, conf) =>
				{
					conf.MinimumLevel.Information()
						.WriteTo.Console()
						.MinimumLevel.Information();
				});

				var botConfigSection = builder.Configuration.GetSection("BotConfiguration");
				builder.Services.Configure<BotConfiguration>(botConfigSection);
				builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(
					httpClient => new TelegramBotClient(botConfigSection.Get<BotConfiguration>()!.BotToken, httpClient));
				builder.Services.AddSingleton<UpdateHandler>();
				builder.Services.AddControllers().AddNewtonsoftJson();
				//builder.Services.ConfigureTelegramBotMvc();


				builder.Services.AddEndpointsApiExplorer();
				builder.Services.AddSwaggerGen();

				builder.Services.AddDbContext<DiplomDbContext>(options =>
					options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreeSqlConnectionString") ?? throw new EmptyValueConnectionStringException("Connection string not found!")));
				builder.Services.AddScoped(typeof(IRepository<>), typeof(EFCoreRepository<>));
				builder.Services.AddScoped<IUserCreatedEventRepository, UserCreatedEventRepository>();
				builder.Services.AddScoped<ITelegramUserRepository, TelegramUserRepository>();
				builder.Services.AddScoped<IScienceEventRepository, ScienceEventRepository>();
				builder.Services.AddScoped<IPasswordHasher<TelegramUser>, PasswordHasher<TelegramUser>>();
				builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();



				builder.Services.AddCors();
				builder.Services.AddHttpLogging(options =>
				{
					options.LoggingFields = HttpLoggingFields.RequestHeaders
											| HttpLoggingFields.ResponseHeaders
											| HttpLoggingFields.RequestBody
											| HttpLoggingFields.ResponseBody;
				});
				var app = builder.Build();
				app.UseHttpsRedirection();
				app.UseHttpLogging();
				app.UseCors(policy =>
				{
					policy
						.AllowAnyMethod()
						.AllowAnyOrigin()
						.AllowAnyHeader();
				});
				app.UseAuthentication();
				app.UseAuthorization();
				app.UseSwagger();
				app.UseSwaggerUI(options =>
				{
					options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
				});
				app.MapControllers();
				await app.RunAsync();
			}
			catch (Exception ex)
			{
				Log.Fatal(ex, "Unexpected error");
			}
			finally
			{
				Log.Information("Server shutting down");
				await Log.CloseAndFlushAsync();
			}
		}
	}
}