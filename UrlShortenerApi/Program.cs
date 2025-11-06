using FluentValidation;
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

// --- Redis Cache Code ---
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "UrlShortener_";
});

// --- Register Your Service ---
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();

// --- Add Fluent Validation Services ---
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

// --- 2. TELL YOUR APP TO USE THE CORS POLICY ---
app.UseCors();

// --- "CREATE LINK" API ENDPOINT ---
app.MapPost("/api/shorten",
    async (
        CreateShortUrlRequest request,
        IUrlShortenerService service,
        IValidator<CreateShortUrlRequest> validator,
        HttpContext httpContext
    ) =>
    {
        // Run the validator
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        // --- THIS IS THE FIX ---
        // We now pass all 3 arguments to the service
        var response = await service.CreateShortUrlAsync(
            request.LongUrl,
            httpContext.Request.Scheme,
            httpContext.Request.Host.ToString()
        );
        // --- END OF FIX ---

        return Results.Ok(response); // This now returns the full object { shortUrl, qrCodeBase64 }
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