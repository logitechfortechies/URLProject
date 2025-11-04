using Microsoft.EntityFrameworkCore;
using UrlShortener.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- Database Code ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- Swagger Code ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- THIS IS THE NEW MIGRATION CODE ---
// Automatically run database migrations when the app starts
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    // This command creates/updates the database tables
    await dbContext.Database.MigrateAsync();
}
// --- END OF NEW MIGRATION CODE ---


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapGet("/", () => "Hello World! The database is connected AND migrated!");

app.Run();