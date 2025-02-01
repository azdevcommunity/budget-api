using BudgetApi.Entities;

namespace BudgetApi.Dto.Customer;

public class CustomerResponse
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }
    
    public string? Address { get; set; }

    public decimal TotalDebt { get; set; }

    // public List<DebtEvent> DebtEvents { get; set; } = new();
}

public class CustomerDebtResponse
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }
    
    public string? Address { get; set; }

    public decimal TotalDebt { get; set; }

    public List<DebtEvent> DebtEvents { get; set; } = new();
}