using System.ComponentModel.DataAnnotations;

namespace BudgetApi.Dto.Customer;

public class CustomerUpdateRequest  
{
    [Required(ErrorMessage = "Customer name cannot be null")]
    public string Name { get; set; }

    public string? Description { get; set; }
    
    public string? Address { get; set; }
}