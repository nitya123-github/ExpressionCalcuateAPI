using Microsoft.AspNetCore.Mvc;
public async Task<IActionResult> Calculate([FromBody] string expression)
{
if (string.IsNullOrWhiteSpace(expression))
return BadRequest(new { error = "Expression is required in request body." });


expression = expression.Trim();


if (expression.Length > 500)
return BadRequest(new { error = "Expression too long (max 500 chars)." });


if (!AllowedExpr.IsMatch(expression))
return BadRequest(new { error = "Expression contains invalid characters. Only digits, whitespace, parentheses and operators +-*/ . are allowed." });


try
{
var expr = new Expression(expression, EvaluateOptions.IgnoreCase);
var raw = expr.Evaluate();


decimal result = raw switch
{
int i => Convert.ToDecimal(i),
long l => Convert.ToDecimal(l),
double d => Convert.ToDecimal(d),
float f => Convert.ToDecimal(f),
decimal m => m,
_ => Convert.ToDecimal(raw)
};


var record = new ExpressionHistory
{
Expression = expression,
Result = result,
CreatedDate = DateTime.UtcNow
};


_db.ExpressionHistories.Add(record);
await _db.SaveChangesAsync();


return Ok(new { expression = record.Expression, result = record.Result, id = record.Id });
}
catch (EvaluationException e)
{
_logger.LogWarning(e, "Evaluation error for expression: {expr}", expression);
return BadRequest(new { error = "Invalid expression: " + e.Message });
}
catch (Exception e)
{
_logger.LogError(e, "Unexpected error while evaluating expression: {expr}", expression);
return StatusCode(500, new { error = "Internal server error" });
}
}


[HttpGet]
public async Task<IActionResult> GetByResult([FromQuery] decimal? result)
{
if (result == null)
return BadRequest(new { error = "Provide 'result' query parameter, e.g. ?result=15" });


var list = await _db.ExpressionHistories
.Where(x => x.Result == result.Value)
.OrderByDescending(x => x.CreatedDate)
.ToListAsync();


return Ok(list);
}
}
}
