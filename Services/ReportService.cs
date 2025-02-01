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

        var transactions = await queryable
            .Include(x => x.Customer)
            .Select(d => new
            {  
                d.EventType,
                d.Amount,
                PaymentDate = d.CreatedAt,
                d.Customer.Name,
                d.Customer.Address,
                d.Customer.Description,
                d.Customer.TotalDebt,
                CustomerId = d.Customer.Id,
                TransactionId = d.Id
            })
            .ToListAsync();

        // Müşterileri gruplandır
        var groupedCustomers = transactions
            .GroupBy(x => x.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                Name = g.First().Name,
                Address = g.First().Address,
                Description = g.First().Description,
                TotalDebt = g.First().TotalDebt,
                Transactions = g.Select(t => new
                {
                    t.TransactionId,
                    t.EventType,
                    t.Amount,
                    t.PaymentDate
                }).ToList()
            })
            .ToList();

        return new
        {
            TotalPaid = transactions.Where(x => x.EventType == DebtEventType.Paid).Sum(x => x.Amount),
            TotalDebt = transactions.Where(x => x.EventType == DebtEventType.AddDebt).Sum(x => x.Amount),
            Customers = groupedCustomers // Tekil müşteri listesi, içinde transactions ile birlikte
        };
    }
}