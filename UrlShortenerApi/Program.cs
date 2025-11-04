using Microsoft.EntityFrameworkCore;
using UrlShortener.Infrastructure; // <-- Add this to use your new file

var builder = WebApplication.CreateBuilder(args);

// --- THIS IS THE NEW DATABASE CODE ---
// 1. Get the connection string from the Environment Variable you set on Render.
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

// 2. Add the Database Context to your app's services.
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));
// --- END OF NEW DATABASE CODE ---


// This is your .NET 8.0 Swagger/OpenAPI code
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// This is your "Hello World" endpoint. 
app.MapGet("/", () => "Hello World! The database is now connected.");

app.Run();