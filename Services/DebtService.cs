using AzDev.Core.DependencyInjection.Attributes;
using BudgetApi.Context;
using BudgetApi.Dto.Debt;
using BudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetApi.Services;

[Component]
public class DebtService(BudgetDbContext context, CustomerService customerService)
{
    // Borç ekleme
    public async Task<bool> AddAsync(int customerId, DebtRequest debt)
    {
        var customer = await customerService.FindByIdAsync(customerId);


        customer.TotalDebt += debt.Amount;

        var debtEvent = new DebtEvent
        {
            CustomerId = customerId,
            Amount = debt.Amount,
            EventType = DebtEventType.AddDebt,
            TotalDebt = customer.TotalDebt
        };

        customer.DebtEvents.Add(debtEvent);
        await context.DebtEvents.AddAsync(debtEvent);
        return await context.SaveChangesAsync() > 0;
    }

    // Borç ödeme
    public async Task<bool> PayDebt(int customerId, DebtRequest debt)
    {
        var customer = await customerService.FindByIdAsync(customerId);

        if (customer.TotalDebt < debt.Amount)
        {
            throw new Exception("Amount cannotbe greater than debt");
        }

        customer.TotalDebt -= debt.Amount;

        var debtEvent = new DebtEvent
        {
            CustomerId = customerId,
            Amount = debt.Amount,
            EventType = DebtEventType.Paid,
            TotalDebt = customer.TotalDebt
        };


        customer.DebtEvents.Add(debtEvent);
        await context.DebtEvents.AddAsync(debtEvent);
        return await context.SaveChangesAsync() > 0;
    }

    // Müşterinin toplam borcunu getir
    public async Task<decimal> GetTotalDebt(int customerId)
    {
        return await context.Customers
            .Where(c => c.Id == customerId)
            .Select(c => c.TotalDebt)
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<DebtEvent>> GetDebtEvents(int customerId)
    {
        return await context.DebtEvents
            .Where(d => d.CustomerId == customerId)
            .Include(d => d.EventType)
            .Include(d => d.Customer)
            .ToListAsync();
    }
}