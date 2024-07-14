using Domain.Entities;
using Microsoft.EntityFrameworkCore;


namespace DiplomProject.Server.DbContexts
{
	public class DiplomDbContext : DbContext
	{
		public DbSet<UserCreatedEvent> UserCreatedEvents => Set<UserCreatedEvent>();
		public DbSet<TelegramUser> TelegramUsers => Set<TelegramUser>();
		public DbSet<ScienceEvent> ScienceEvents => Set<ScienceEvent>();
		public DbSet<Password> Passwords => Set<Password>();
		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<UserCreatedEvent>()
			.HasOne(e => e.TgUser)
			.WithMany(u => u.UserCreatedEvents)
			.OnDelete(DeleteBehavior.Cascade);

			base.OnModelCreating(modelBuilder);
		}
		public DiplomDbContext(DbContextOptions<DiplomDbContext> options) : base(options)
		{
		}
	}
}
