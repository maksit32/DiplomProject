using DiplomProject.Server.Configurations;
using DiplomProject.Server.DbContexts;
using DiplomProject.Server.Repositories;
using DiplomProject.Server.Services;
using Domain.Entities;
using Domain.Exceptions;
using Domain.Models;
using Domain.Repositories.Interfaces;
using Domain.Services.Interfaces;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.HttpLogging;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json.Linq;
using Serilog;
using Serilog.Core;
using System.Text;
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
				//добавляем внешний json файл (путь из секретов)
				builder.Configuration.AddJsonFile(builder.Configuration["TelegramTextConstantsPath"], optional: true, reloadOnChange: true);

				//регистрируем конфиг jwt
				var jwtConfig = builder.Configuration
				.GetRequiredSection("Jwt")
				.Get<JwtConfig>();
				builder.Services.Configure<JwtConfig>(config =>
				{
					config.SigningKey = jwtConfig.SigningKey;
					config.Audience = jwtConfig.Audience;
					config.Issuer = jwtConfig.Issuer;
					config.LifeTime = jwtConfig.LifeTime;
				});

				var botConfigSection = builder.Configuration.GetSection("BotConfiguration");
				builder.Services.Configure<BotConfiguration>(botConfigSection);
				builder.Services.AddHttpClient("tgwebhook").RemoveAllLoggers().AddTypedClient<ITelegramBotClient>(
					httpClient => new TelegramBotClient(botConfigSection.Get<BotConfiguration>()!.BotToken, httpClient));
				builder.Services.AddScoped<UpdateHandler>();
				builder.Services.AddControllers().AddNewtonsoftJson();

				builder.Services.AddEndpointsApiExplorer();
				builder.Services.AddSwaggerGen(options =>
				{
					options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
					{
						Name = "Authorization",
						Type = SecuritySchemeType.ApiKey,
						Scheme = "Bearer",
						BearerFormat = "JWT",
						In = ParameterLocation.Header,
						Description = "JWT Authorization header using the Bearer scheme. " +
									  "\r\n\r\n Enter 'Bearer' [space] and then your token in the text input below." +
										 "\r\n\r\nExample: \"Bearer {token}\"",
					});
					options.AddSecurityRequirement(new OpenApiSecurityRequirement
				{
					{
						new OpenApiSecurityScheme
						{
							Reference = new OpenApiReference
							{
								Type = ReferenceType.SecurityScheme,
								Id = "Bearer"
							}
						},
						Array.Empty<string>()
					}
				});
				});

				//из secrets.json автоматом
				builder.Services.AddSingleton<IConfiguration>(builder.Configuration);
				builder.Services.AddDbContext<DiplomDbContext>(options =>
					options.UseNpgsql(builder.Configuration.GetConnectionString("PostgreeSqlConnectionString") ?? throw new EmptyValueConnectionStringException("Connection string not found!")));
				builder.Services.AddScoped(typeof(IRepository<>), typeof(EFCoreRepository<>));
				builder.Services.AddScoped<IUserCreatedEventRepository, UserCreatedEventRepository>();
				builder.Services.AddScoped<ITelegramUserRepository, TelegramUserRepository>();
				builder.Services.AddScoped<IScienceEventRepository, ScienceEventRepository>();
				builder.Services.AddScoped<IPasswordHasherService, PasswordHasherService>();
				builder.Services.AddScoped<INotifyService, NotifyService>();
				builder.Services.AddScoped<ITgUserService, TgUserService>();
				builder.Services.AddScoped<IScienceEventService, ScienceEventService>();
				builder.Services.AddScoped<IUserCreatedEventService, UserCreatedEventService>();
				builder.Services.AddScoped<IJwtService, JwtService>();
				builder.Services.AddScoped<IAuthenticationService, AuthenticationService>();
				builder.Services.AddScoped<IValidationService, ValidationService>();
				builder.Services.AddScoped<IDocumentService, WordDocumentService>();
				builder.Services.AddSingleton<IFillDataService, FillDocxOpenXMLDataService>();



				builder.Services.AddCors();
				builder.Services.AddHttpLogging(options =>
				{
					options.LoggingFields = HttpLoggingFields.RequestHeaders
											| HttpLoggingFields.ResponseHeaders
											| HttpLoggingFields.RequestBody
											| HttpLoggingFields.ResponseBody;
				});
				builder.Services.AddAuthentication(options =>
				{
					options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultSignInScheme = JwtBearerDefaults.AuthenticationScheme;
					options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
				})
								.AddJwtBearer(options =>
								{
									options.TokenValidationParameters = new TokenValidationParameters
									{
										IssuerSigningKey = new SymmetricSecurityKey(jwtConfig.SigningKeyBytes),
										ValidateIssuerSigningKey = true,
										ValidateLifetime = true,
										RequireExpirationTime = true,
										RequireSignedTokens = true,
										ValidateAudience = true,
										ValidateIssuer = true,
										ValidAudiences = new[] { jwtConfig.Audience },
										ValidIssuer = jwtConfig.Issuer
									};
								});
				builder.Services.AddAuthorization();

				var app = builder.Build();
				app.UseHttpsRedirection();
				app.UseHttpLogging();
				app.UseRouting();
				app.UseCors(policy =>
				{
					policy
						.AllowAnyMethod()
						.WithOrigins("http://localhost:8443", "http://localhost:5173/", "http://localhost:5174/")
						.AllowAnyHeader()
						.AllowCredentials();
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