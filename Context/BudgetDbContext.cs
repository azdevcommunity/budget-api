using BudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetApi.Context;

public class BudgetDbContext(DbContextOptions<BudgetDbContext> options, IConfiguration configuration)
    : DbContext(options)
{
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<DebtEvent> DebtEvents { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema(configuration["Database:Scheme"]);
        
        modelBuilder.Entity<DebtEvent>()
            .HasOne(d => d.Customer)
            .WithMany(c => c.DebtEvents)
            .HasForeignKey(d => d.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}