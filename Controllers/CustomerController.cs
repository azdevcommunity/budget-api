using Asp.Versioning;
using BudgetApi.Dto.Customer;
using BudgetApi.Dto.Debt;
using BudgetApi.Services;
using Microsoft.AspNetCore.Mvc;

namespace BudgetApi.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class CustomerController(CustomerService customerService, DebtService debtService) : ControllerBase
{
    [HttpPost]
    public async Task<IActionResult> AddCustomer([FromBody] CustomerCreateRequest request)
    {
        return Ok(await customerService.AddAsync(request));
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetCustomer(int id)
    {
        var customer = await customerService.GetByIdAsync(id);
        if (customer == null) return NotFound();
        return Ok(customer);
    }

    [HttpGet]
    public async Task<IActionResult> GetAllCustomers()
    {
        var customers = await customerService.GetAllAsync();
        return Ok(customers);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateCustomer(int id, [FromBody] CustomerUpdateRequest request)
    {
        var updated = await customerService.UpdateAsync(request, id);
        return updated ? Ok() : BadRequest();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCustomer(int id)
    {
        return Ok(await customerService.DeleteAsync(id));
    }

    // Borç ekleme
    [HttpPost("{id}/add-debt")]
    public async Task<IActionResult> AddDebt(int id, [FromBody] DebtRequest debt)
    {
        return Ok(await debtService.AddAsync(id, debt));
    }

    // Borç ödeme
    [HttpPost("{id}/pay-debt")]
    public async Task<IActionResult> PayDebt(int id, [FromBody] DebtRequest debt)
    {
        return Ok(await debtService.PayDebt(id, debt));
    }
    
    [HttpGet("{id}/last-payment")]
    public async Task<IActionResult> GetLastPayment(int id)
    {
       return Ok(await debtService.GetLastPayment(id));
    }
    
    // Borç ödeme
    [HttpPost("{id}/reverse-debt-event")]
    public async Task<IActionResult> ReverseDebtEvent(int id)
    {
        return Ok(await debtService.ReverseDebt(id));
    } 
    

}