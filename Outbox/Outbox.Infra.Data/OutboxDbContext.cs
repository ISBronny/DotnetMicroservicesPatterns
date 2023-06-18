using Microsoft.EntityFrameworkCore;
using Outbox.Domain.Models;

namespace Outbox.Infra.Data;

public class OutboxDbContext : DbContext
{
	public OutboxDbContext(DbContextOptions<OutboxDbContext> options) : base(options)
	{
		Database.EnsureCreated();
	}

	protected override void OnModelCreating(ModelBuilder modelBuilder)
	{
		base.OnModelCreating(modelBuilder);

		modelBuilder.Entity<Order>()
			.ToTable("orders")
			.HasKey(o => o.Id);
	}

	public DbSet<Order> Orders { get; set; }
}