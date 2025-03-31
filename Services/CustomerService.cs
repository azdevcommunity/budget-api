using AutoMapper;
using AutoMapper.QueryableExtensions;
using AzDev.Core.DependencyInjection.Attributes;
using BudgetApi.Context;
using BudgetApi.Dto.Customer;
using BudgetApi.Entities;
using Microsoft.EntityFrameworkCore;

namespace BudgetApi.Services;

[Component]
public class CustomerService(BudgetDbContext context, IMapper mapper)
{
    public async Task<CustomerResponse> AddAsync(CustomerCreateRequest request)
    {
        var dublicate = await context.Customers.AnyAsync(x => x.Name == request.Name);

        if (dublicate)
        {
            throw new Exception($"Customer with name {request.Name} already exists");
        }

        Customer customer = mapper.Map<Customer>(request);


        if (customer.TotalDebt > 0)
        {
            customer.CurrentDebt = customer.TotalDebt;
            
            customer.DebtEvents.Add(new DebtEvent
            {
                Amount = customer.TotalDebt,
                TotalDebt = customer.TotalDebt,
                EventType = DebtEventType.AddDebt,
                CreatedAt = DateTime.UtcNow
            });
        }
        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();
        return mapper.Map<CustomerResponse>(customer);
    }

    public async Task<CustomerDebtResponse> GetByIdAsync(int id)
    {
        var customer = await context.Customers
            .AsNoTracking()
            .Include(c => c.DebtEvents)
            .Where(c => c.Id == id)
            .ProjectTo<CustomerDebtResponse>(mapper.ConfigurationProvider)
            .FirstOrDefaultAsync();

        return customer ?? throw new Exception($"Customer with id {id} not found");
    }

    public async Task<Customer> FindByIdAsync(int id)
    {
        var customer = await context.Customers.FindAsync(id)
                       ?? throw new Exception($"Customer with id {id} not found");

        return customer;
    }

    public async Task<object> GetAllAsync()
    {
        return await context.Customers
            .AsNoTracking()
            // .Include(c => c.DebtEvents)
            .ProjectTo<CustomerResponse>(mapper.ConfigurationProvider)
            .ToListAsync();
    }

    public async Task<bool> UpdateAsync(CustomerUpdateRequest request, int id)
    {
        Customer customer = await context.Customers.FindAsync(id)
                            ?? throw new Exception($"Customer with id {id} not found");

        var dublicate = await context.Customers.AnyAsync(x => x.Name == request.Name && x.Id != id);


        if (dublicate)
        {
            throw new Exception($"Customer with name {request.Name} already exists");
        }

        Customer updatedCustomer = mapper.Map(request, customer);

        context.Customers.Update(updatedCustomer);

        return await context.SaveChangesAsync() > 0;
    }

    public async Task<bool> DeleteAsync(int id)
    {
        Customer customer = await context.Customers.FindAsync(id)
                            ?? throw new Exception($"Customer with id {id} not found");

        customer.IsActive = false;
        return await context.SaveChangesAsync() > 0;
    }
}