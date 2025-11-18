# ExpressionCalculatorAPI


Simple .NET 8 Web API that evaluates math expressions, stores request/response in a SQLite DB, and lets you query expressions by the result.


## Files included
- Controllers/CalculateController.cs
- Data/AppDbContext.cs
- Models/ExpressionHistory.cs
- Program.cs
- appsettings.json
- ExpressionCalculatorAPI.csproj


## Run locally or in Codespaces
1. Open in Codespaces or clone locally.
2. Restore packages: `dotnet restore`
3. From project folder: `dotnet run`
4. Open Swagger at `/swagger` to test endpoints.


## Endpoints
- `POST /api/calculate` — body: raw JSON string, e.g. `"3+4*6-12"`
- `GET /api/calculate?result={value}` — returns expressions with that result


## Notes
- Uses SQLite `expressions.db` in project folder.
- Input is validated to allow digits, whitespace, parentheses, and operators `+-*/.` only. If you need functions (sin, cos) remove validation or extend.
