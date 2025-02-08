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
        customer.CurrentDebt += debt.Amount;

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

        if (customer.CurrentDebt < debt.Amount)
        {
            throw new Exception("Amount cannotbe greater than debt");
        }

        customer.CurrentDebt -= debt.Amount;
        customer.TotalPayment += debt.Amount;

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

    public async Task<object?> GetLastPayment(int customerId)
    {
        return await context.Customers
            .Where(c => c.Id == customerId)
            .Select(c => new
            {
                c.TotalDebt,
                LastPayment = c.DebtEvents
                    .Where(d => d.EventType == DebtEventType.Paid)
                    .OrderByDescending(d => d.CreatedAt)
                    .Select(d => new
                    {
                        Amount = (decimal?)d.Amount,
                        PaymentDate = (DateTime?)d.CreatedAt
                    })
                    .FirstOrDefault()
            })
            .FirstOrDefaultAsync();
    }

    public async Task<IEnumerable<DebtEvent>> GetDebtEvents(int customerId)
    {
        return await context.DebtEvents.AsNoTracking()
            .Where(d => d.CustomerId == customerId)
            .Include(d => d.EventType)
            .Include(d => d.Customer)
            .ToListAsync();
    }

    public async Task<object?> ReverseDebt(int id)
    {
        DebtEvent debtEvent = await context.DebtEvents
                                  .IgnoreQueryFilters()
                                  .FirstOrDefaultAsync(d => d.Id == id)
                              ?? throw new Exception("Debt event not found");

      


        Customer customer = await customerService.FindByIdAsync(debtEvent.CustomerId);
        
        if (debtEvent.EventType == DebtEventType.Paid)
        {
            customer.CurrentDebt += debtEvent.Amount;
            customer.TotalDebt += debtEvent.Amount;
            customer.TotalPayment -= debtEvent.Amount;
        }
        else
        {
            customer.CurrentDebt -= debtEvent.Amount;
            customer.TotalDebt -= debtEvent.Amount;
        }
        
        debtEvent.Reversed = true;

        return await context.SaveChangesAsync() > 0;
    }
}