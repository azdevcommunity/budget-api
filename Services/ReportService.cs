using AzDev.Core.DependencyInjection.Attributes;
using Azure;
using BudgetApi.Context;
using BudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetApi.Services;

[Component]
public class ReportService(BudgetDbContext context, CustomerService customerService)
{
    public async Task<dynamic> GetCashFlow(DateTime startDate, DateTime endDate, int customerId)
    {
        Customer customer = await customerService.FindByIdAsync(customerId);

        var transactions = await context.DebtEvents
            .AsNoTracking()
            .Where(x => x.CustomerId == customerId)
            .Where(d => d.CreatedAt >= startDate && d.CreatedAt < endDate)
            .Distinct()
            .Select(d => new
            {
                d.Id,
                d.EventType,
                d.Amount,
                PaymentDate = d.CreatedAt,
            })
            .ToListAsync();

        return  new
        {
            TotalPaid = customer.TotalPayment,
            customer.CurrentDebt,
            customer.TotalDebt,
            customer.Name,
            customer.Address,
            Transactions = transactions
        };
    }

    public async Task<object> GetTotalCashFlow(DateTime startDate, DateTime endDate, int customerId)
    {
        var queryable = context.DebtEvents
            .AsNoTracking()
            .Include(x=>x.Customer)
            .Where(d => d.CreatedAt >= startDate && d.CreatedAt < endDate && d.Customer.IsActive);

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