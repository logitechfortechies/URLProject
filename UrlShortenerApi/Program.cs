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
    async (
        CreateShortUrlRequest request,
        IUrlShortenerService service,
        IValidator<CreateShortUrlRequest> validator, // <-- 3. INJECT THE VALIDATOR
        HttpContext httpContext
    ) =>
    {
        // 4. RUN THE VALIDATOR
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            // If validation fails, return a 400 Bad Request with the errors
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        // --- END OF VALIDATION CODE ---

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

app.MapGet("/", () => "Hello World! The database is connected AND migrated!");

app.Run();

// --- Request/Response Objects ---
public record CreateShortUrlRequest(string LongUrl);
public record CreateShortUrlResponse(string ShortUrl);