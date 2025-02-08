namespace BudgetApi.Entities;

public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }
    
    public string? Address { get; set; }
    
    public decimal CurrentDebt { get; set; }

    public decimal TotalDebt { get; set; }
    
    public decimal TotalPayment { get; set; }
    
    public bool IsActive { get; set; }

    public List<DebtEvent> DebtEvents { get; set; } = new();
}