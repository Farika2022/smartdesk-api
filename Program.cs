
 using SmartDesk.Api.Data;
 using Microsoft.EntityFrameworkCore;
 using Microsoft.AspNetCore.Authentication.JwtBearer;
 using Microsoft.IdentityModel.Tokens;
 using System.Net.Http.Headers;
 using System.Text;
 using SmartDesk.Api.Services;
// WebApplication.CreateBuilder => sets up everything .NET needs to run:
// dependency injection, configuration, logging.
// builder = the setup phase. app = the running phase.
var builder = WebApplication.CreateBuilder(args);


//AddControllers =>  Tells .NET: I have controller classes. Find them and wire them up.
// Without this, TicketsController is never discovered.
builder.Services.AddControllers();


builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer=true,
                ValidateAudience=true,
                ValidateLifetime=true,
                ValidateIssuerSigningKey=true,
                ValidIssuer = builder.Configuration["Jwt:Issuer"],
                ValidAudience= builder.Configuration["Jwt:Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(
                    Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"]!)
                )
            };
    });

// AddDbContext => Registers SmartDeskContext with dependency injection.
// Every controller that needs the DB gets it automatically.

// AddDbContext => Tells Entity Framework: use PostgreSQL as the database provider.
// The connection string tells it where the database is.

builder.Services.AddDbContext<SmartDeskContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


// GROQ_API_KEY is read from the process environment ( a free key )
// If it's missing, the HttpClient is still built —
// requests just come back unauthorized, which TicketClassifierService catches and
// treats like any other classification failure.
var groqApiKey = Environment.GetEnvironmentVariable("GROQ_API_KEY");
builder.Services.AddHttpClient<ITicketClassifierService, TicketClassifierService>(client =>
{
    client.BaseAddress = new Uri("https://api.groq.com/openai/v1/");
    if (!string.IsNullOrEmpty(groqApiKey))
    {
        client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", groqApiKey);
    }
});

// CORS => the browser blocks requests between different origins by default.
// CORS tells the browser: it is safe to allow the frontend to call this API.
// ALLOWED_ORIGINS is a comma-separated list (e.g. the deployed Vercel URL in
// production); it always includes the local Vite dev server so local dev keeps working.
var allowedOrigins = new[] { "http://localhost:5173" }
    .Concat((Environment.GetEnvironmentVariable("ALLOWED_ORIGINS") ?? "")
        .Split(',', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries))
    .Distinct()
    .ToArray();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowReact", policy =>
    {
        policy.WithOrigins(allowedOrigins)
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

var app = builder.Build();

//  UseCors before MapControllers => Middleware runs in order. CORS must be checked before
// the request reaches the controller. Order matters in .NET.
app.UseCors("AllowReact");
app.UseAuthentication();
app.UseAuthorization();
// MapControllers => Connects the URL routes to the controller methods.
// GET /api/tickets → TicketsController.GetAll()
// Without this, the endpoints never get registered.
app.MapControllers();
app.UseAuthentication();
app.UseAuthorization();
// Railway does not have a terminal to run dotnet ef manually.
// This applies any pending migrations automatically when the app starts.
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<SmartDeskContext>();
    db.Database.Migrate();
}
app.Run();
