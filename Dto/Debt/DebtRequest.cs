using System.ComponentModel.DataAnnotations;

namespace BudgetApi.Dto.Debt;

public class DebtRequest
{
    [Required(ErrorMessage = "Amount cannot be null or empty")]
    [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
    public decimal Amount { get; set; }
}