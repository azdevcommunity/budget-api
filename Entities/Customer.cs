namespace BudgetApi.Entities;

public class Customer
{
    public int Id { get; set; }

    public string Name { get; set; }

    public string? Description { get; set; }

    public decimal TotalDebt { get; set; }

    public List<DebtEvent> DebtEvents { get; set; } = new();
}