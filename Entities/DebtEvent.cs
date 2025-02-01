namespace BudgetApi.Entities;

public class DebtEvent
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public Customer Customer { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalDebt { get; set; } 
    public DebtEventType EventType { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}