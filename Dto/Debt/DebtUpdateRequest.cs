using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using BudgetApi.Entities;

namespace BudgetApi.Dto.Debt;

public class DebtUpdateRequest
{
    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }
    public DebtEventType EventType { get; set; }
    
    [JsonPropertyName("debtDate")]
    public DateTime CreatedAt { get; set; }
}