using Microsoft.EntityFrameworkCore;
using ExpressionCalculatorAPI.Data;


var builder = WebApplication.CreateBuilder(args);


// Add services
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();


builder.Services.AddDbContext<AppDbContext>(options =>
options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));


var app = builder.Build();


// Auto-migrate DB on startup
using (var scope = app.Services.CreateScope())
{
var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
db.Database.Migrate();
}


if (app.Environment.IsDevelopment())
{
app.UseSwagger();
app.UseSwaggerUI();
}


app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();
