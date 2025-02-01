using AzDev.Core.DependencyInjection.Attributes;
using Azure;
using BudgetApi.Context;
using BudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetApi.Services;

[Component]
public class ReportService(BudgetDbContext context)
{
    public async Task<object> GetCashFlow(DateTime startDate, DateTime endDate)
    {
        var transactions = await context.DebtEvents
            .AsNoTracking()
            .Where(d => d.CreatedAt >= startDate && d.CreatedAt < endDate)
            .GroupBy(d => d.EventType)
            .Select(g => new
            {
                EventType = g.Key,
                TotalAmount = g.Sum(d => d.Amount)
            })
            .ToListAsync();

        return new
        {
            TotalPaid = transactions.Where(x => x.EventType == DebtEventType.Paid).Sum(x => x.TotalAmount),
            TotalDebt = transactions.Where(x => x.EventType == DebtEventType.AddDebt).Sum(x => x.TotalAmount),
            Transactions = transactions
        };
    }
}