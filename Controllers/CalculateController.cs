using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ExpressionCalculatorAPI.Data;
using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using ExpressionCalculatorAPI.Data;
using ExpressionCalculatorAPI.Models;
using NCalc;

namespace ExpressionCalculatorAPI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CalculateController : ControllerBase
    {
        private readonly AppDbContext _db;
        private readonly ILogger<CalculateController> _logger;

        public CalculateController(AppDbContext db, ILogger<CalculateController> logger)
        {
            _db = db;
            _logger = logger;
        }

        public class ExpressionRequest
        {
            public string Expression { get; set; } = string.Empty;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ExpressionRequest request)
        {
            var expression = request?.Expression ?? string.Empty;
            if (string.IsNullOrWhiteSpace(expression))
                return BadRequest(new { error = "Provide an 'expression' in the request body." });

            foreach (var ch in expression)
            {
                if (char.IsDigit(ch) || char.IsWhiteSpace(ch) || ch == '(' || ch == ')' || ch == '+' || ch == '-' || ch == '*' || ch == '/' || ch == '.')
                    continue;

                return BadRequest(new { error = "Expression contains invalid characters. Only digits, whitespace, parentheses and operators +-*/ . are allowed." });
            }

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

        [HttpGet("by-result")]
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
