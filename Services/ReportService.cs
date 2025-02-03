using AzDev.Core.DependencyInjection.Attributes;
using Azure;
using BudgetApi.Context;
using BudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetApi.Services;

[Component]
public class ReportService(BudgetDbContext context)
{
    public async Task<object> GetCashFlow(DateTime startDate, DateTime endDate, int customerId)
    {
        var queryable = context.DebtEvents
            .AsNoTracking()
            .Where(d => d.CreatedAt >= startDate && d.CreatedAt < endDate);

        if (customerId != 0)
        {
            queryable = queryable
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId);
        }

        var transactions = await queryable
            .AsNoTracking()
            .Include(x => x.Customer)
            .Distinct()
            .Select(d => new
            {  
                d.EventType, 
                d.Amount,
                PaymentDate = d.CreatedAt,
            })
           
            .ToListAsync();

        return new
        {
            TotalPaid = transactions.Where(x => x.EventType == DebtEventType.Paid).Sum(x => x.Amount),
            TotalDebt = transactions.Where(x => x.EventType == DebtEventType.AddDebt).Sum(x => x.Amount),
            Transactions = transactions
        };
    }
    public async Task<object> GetTotalCashFlow(DateTime startDate, DateTime endDate, int customerId)
    {
        var queryable = context.DebtEvents
            .AsNoTracking()
            .Where(d => d.CreatedAt >= startDate && d.CreatedAt < endDate);

        if (customerId != 0)
        {
            queryable = queryable
                .AsNoTracking()
                .Where(x => x.CustomerId == customerId);
        }

        var totalPaid = await queryable
            .AsNoTracking()
            .Where(d => d.EventType == DebtEventType.Paid)
            .SumAsync(d => d.Amount); // SQL'de SUM çalıştırılır

        var totalDebt = await queryable
            .AsNoTracking()
            .Where(d => d.EventType == DebtEventType.AddDebt)
            .SumAsync(d => d.Amount); // SQL'de SUM çalıştırılır

        return new
        {
            TotalPaid = totalPaid,
            TotalDebt = totalDebt
        };
    }
}