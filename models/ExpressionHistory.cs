using System;


namespace ExpressionCalculatorAPI.Models
{
public class ExpressionHistory
{
public int Id { get; set; }
public string Expression { get; set; } = string.Empty;
public decimal Result { get; set; }
public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
}
}
