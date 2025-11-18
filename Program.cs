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


// Apply any pending migrations at startup so the database schema
// matches the code model. This is the recommended production approach.
using (var scope = app.Services.CreateScope())
{
	var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
	db.Database.Migrate();
}


// For this development environment: enable Swagger UI so the root can redirect to it.
app.UseSwagger();
app.UseSwaggerUI();

// Skip HTTPS redirection in the dev forwarded environment to avoid redirect issues.
// app.UseHttpsRedirection();
app.UseAuthorization();

// Root: redirect to Swagger UI so visiting the forwarded domain shows a useful page.
app.MapGet("/", () => Results.Redirect("/swagger"));

app.MapControllers();
app.Run();
