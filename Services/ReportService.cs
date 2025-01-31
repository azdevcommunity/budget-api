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
        var result = context.DebtEvents
            .Where(d => d.CreatedAt >= startDate && d.CreatedAt < endDate)
            .GroupBy(d => d.EventType)
            .Select(g => new
            {
                EventType = g.Key,
                TotalAmount = g.Sum(d => d.Amount)
            });

        return new
        {
            TotalPaid = await result.Where(x => x.EventType == DebtEventType.Paid)
                .SumAsync(x => x.TotalAmount),
            TotalDebt = await result.Where(x => x.EventType == DebtEventType.AddDebt)
                .SumAsync(x => x.TotalAmount),
            Transactions = await result.ToListAsync()
        };
    }
}