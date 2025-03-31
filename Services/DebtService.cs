using System.Text.Json;
using AzDev.Core.DependencyInjection.Attributes;
using BudgetApi.Context;
using BudgetApi.Dto.Debt;
using BudgetApi.Entities;
using Microsoft.EntityFrameworkCore;
using ILogger = Serilog.ILogger;

namespace BudgetApi.Services;

[Component]
public class DebtService(BudgetDbContext context, CustomerService customerService, ILogger logger)
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

    public async Task<object> GetDebtEvents()
    {
        return await context.DebtEvents.AsNoTracking()
            .Include(d => d.Customer)
            .Where(d => d.Customer.IsActive)
            .Select(de=>new
            {
                de.Amount,
                de.EventType,
                de.TotalDebt,
                de.CreatedAt,
                de.Id,
                Customer = new Customer
                {
                    Id = de.Customer.Id,
                    Name = de.Customer.Name,
                    CurrentDebt = de.Customer.TotalDebt
                },
            })
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

    public async Task<object?> UpdateDebtEvent(int id, DebtUpdateRequest request)
    {
        //serializ

        string v = JsonSerializer.Serialize(request);
        logger.Information(v);
        DebtEvent debtEvent = await context.DebtEvents
                                  .IgnoreQueryFilters()
                                  .FirstOrDefaultAsync(d => d.Id == id)
                              ?? throw new Exception("Debt event not found");

        Customer customer = await customerService.FindByIdAsync(debtEvent.CustomerId);

        decimal previousAmount = debtEvent.Amount;
        DebtEventType previousType = debtEvent.EventType;


        debtEvent.Amount = request.Amount;
        debtEvent.EventType = request.EventType;
        debtEvent.CreatedAt = request.CreatedAt.ToUniversalTime();


        if (previousType == DebtEventType.AddDebt)
        {
            customer.CurrentDebt -= previousAmount;
            customer.TotalDebt -= previousAmount;
        }
        else if (previousType == DebtEventType.Paid)
        {
            customer.TotalPayment -= previousAmount;
            customer.CurrentDebt += previousAmount;
        }


        if (request.EventType == DebtEventType.AddDebt)
        {
            customer.CurrentDebt += request.Amount;
            customer.TotalDebt += request.Amount;
        }
        else if (request.EventType == DebtEventType.Paid)
        {
            customer.TotalPayment += request.Amount;
            customer.CurrentDebt -= request.Amount;
        }

        return await context.SaveChangesAsync() > 0;
    }
}