using Microsoft.EntityFrameworkCore;
using UrlShortener.Application;
using UrlShortener.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

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

// --- Auto-Migration Code ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

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

// ---
// --- 1. THIS IS YOUR NEW "REDIRECT" ENDPOINT ---
// ---
app.MapGet("/{shortCode}", async (string shortCode, IUrlShortenerService service) =>
{
    var longUrl = await service.GetLongUrlAsync(shortCode);

    if (longUrl is null)
    {
        // If the code doesn't exist, return a 404 Not Found
        return Results.NotFound();
    }

    // If it exists, redirect the user to the original long URL
    return Results.Redirect(longUrl, permanent: true);
});

app.Run();

// --- Request/Response Objects ---
public record CreateShortUrlRequest(string LongUrl);
public record CreateShortUrlResponse(string ShortUrl);