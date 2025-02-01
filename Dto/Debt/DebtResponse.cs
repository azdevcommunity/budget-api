using BudgetApi.Entities;
using Newtonsoft.Json;

namespace BudgetApi.Dto.Debt;

public class DebtResponse
{
    public int Id { get; set; }
    public int CustomerId { get; set; }
    public decimal Amount { get; set; }
    public decimal TotalDebt { get; set; }
    public DebtEventType EventType { get; set; }
    
    [JsonProperty("debtDte")] 
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}