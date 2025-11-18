using Microsoft.EntityFrameworkCore;
using ExpressionCalculatorAPI.Models;


namespace ExpressionCalculatorAPI.Data
{
public class AppDbContext : DbContext
{
public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }


public DbSet<ExpressionHistory> ExpressionHistories { get; set; }
}
}
