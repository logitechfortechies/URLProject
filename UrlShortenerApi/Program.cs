using FluentValidation; // <-- 1. ADD THIS
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application;
using UrlShortener.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- CORS Policy ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// --- Database Code ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- Redis Cache Code ---
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "UrlShortener_";
});

// --- Register Your Service ---
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();

// --- 2. ADD FLUENT VALIDATION SERVICES ---
builder.Services.AddValidatorsFromAssemblyContaining<IUrlShortenerService>();

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
app.UseCors();

// --- "CREATE LINK" API ENDPOINT ---
app.MapPost("/api/shorten",
    async (CreateShortUrlRequest request, IUrlShortenerService service, HttpContext httpContext) =>
    {
        // We'll add FluentValidation later
        if (string.IsNullOrEmpty(request.LongUrl) || !Uri.IsWellFormedUriString(request.LongUrl, UriKind.Absolute))
        {
            return Results.BadRequest("Invalid URL.");
        }

        // THIS IS THE CHANGE: Pass the request Scheme and Host to the service
        var response = await service.CreateShortUrlAsync(
            request.LongUrl,
            httpContext.Request.Scheme,
            httpContext.Request.Host.ToString()
        );

        return Results.Ok(response); // This now returns { shortUrl: "...", qrCodeBase64: "..." }
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

app.MapGet("/", () => "Hello World! The database is connected AND migrated!");

app.Run();

