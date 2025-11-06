using Microsoft.EntityFrameworkCore;
using UrlShortener.Application;
using UrlShortener.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- 1. DEFINE YOUR CORS POLICY ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173") // Allow your local Vue app
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- Database Code ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- Register Your Service ---
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();

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

// --- 2. TELL YOUR APP TO USE THE CORS POLICY ---
app.UseCors();

// --- "CREATE LINK" API ENDPOINT ---
app.MapPost("/api/shorten",
    async (CreateShortUrlRequest request, IUrlShortenerService service, HttpContext httpContext) =>
    {
        if (string.IsNullOrEmpty(request.LongUrl) || !Uri.IsWellFormedUriString(request.LongUrl, UriKind.Absolute))
        {
            return Results.BadRequest("Invalid URL.");
        }

        var shortCode = await service.CreateShortUrlAsync(request.LongUrl);
        var shortUrl = $"{httpContext.Request.Scheme}://{httpContext.Request.Host}/{shortCode}";

        return Results.Ok(new CreateShortUrlResponse(shortUrl));
    });

// --- "REDIRECT" ENDPOINT ---
app.MapGet("/{shortCode}", async (string shortCode, IUrlShortenerService service) =>
{
    var longUrl = await service.GetLongUrlAsync(shortCode);

    if (longUrl is null)
    {
        return Results.NotFound();
    }

    return Results.Redirect(longUrl, permanent: true);
});

// --- NEW ROOT ENDPOINT ---
// This will be our new success message
app.MapGet("/", () => "Hello World! The database is connected AND migrated!");

app.Run();

// --- Request/Response Objects ---
public record CreateShortUrlRequest(string LongUrl);
public record CreateShortUrlResponse(string ShortUrl);