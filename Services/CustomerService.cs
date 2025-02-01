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
    public async Task<Customer> AddAsync(CustomerCreateRequest request)
    {
        var dublicate = await context.Customers.AnyAsync(x => x.Name == request.Name);

        if (dublicate)
        {
            throw new Exception($"Customer with name {request.Name} already exists");
        }

        Customer customer = mapper.Map<Customer>(request);
        await context.Customers.AddAsync(customer);
        await context.SaveChangesAsync();
        return customer;
    }

    public async Task<CustomerResponse> GetByIdAsync(int id)
    {
        var customer = await context.Customers
            .AsNoTracking()
            .Include(c => c.DebtEvents)
            .Where(c => c.Id == id)
            .ProjectTo<CustomerResponse>(mapper.ConfigurationProvider)
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
            .Include(c => c.DebtEvents)
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
        var count = await context.DebtEvents.Where(x => x.CustomerId == id).CountAsync();

        if (count > 0)
        {
            throw new Exception($"Customer with id {id} already have debt events");
        }

        Customer customer = await context.Customers.FindAsync(id)
                            ?? throw new Exception($"Customer with id {id} not found");

        context.Customers.Remove(customer);
        return await context.SaveChangesAsync() > 0;
    }
}