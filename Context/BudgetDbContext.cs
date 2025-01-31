using BudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetApi.Context;

public class BudgetDbContext : DbContext
{
    public virtual DbSet<Customer> Customers { get; set; }
    public virtual DbSet<DebtEvent> DebtEvents { get; set; }

    public BudgetDbContext(DbContextOptions<BudgetDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("BudgetManagement");
        modelBuilder.Entity<DebtEvent>()
            .HasOne(d => d.Customer)
            .WithMany(c => c.DebtEvents)
            .HasForeignKey(d => d.CustomerId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}