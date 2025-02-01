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
            queryable = queryable.Where(x => x.CustomerId == customerId);
        }

        var transactions = await queryable.Select(d => new
            {
                d.EventType,
                d.Amount,
                d.CreatedAt
            })
            .ToListAsync();

        return new
        {
            TotalPaid = transactions.Where(x => x.EventType == DebtEventType.Paid).Sum(x => x.Amount),
            TotalDebt = transactions.Where(x => x.EventType == DebtEventType.AddDebt).Sum(x => x.Amount),
            Transactions = transactions
        };
    }
}