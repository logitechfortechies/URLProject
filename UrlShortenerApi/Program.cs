using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application;
using UrlShortener.Infrastructure;
using System.Security.Claims;

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

// --- 2. CONFIGURE DATABASE ---
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseNpgsql(connectionString));

// --- 3. CONFIGURE REDIS CACHE ---
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetConnectionString("Redis");
    options.InstanceName = "UrlShortener_";
});

// --- 4. ADD IDENTITY & AUTHORIZATION SERVICES ---
builder.Services.AddAuthorization();
builder.Services.AddIdentityApiEndpoints<IdentityUser>()
    .AddEntityFrameworkStores<ApplicationDbContext>();

// --- 5. CONFIGURE RATE LIMITING SERVICE ---
builder.Services.AddRateLimiter(options =>
{
    options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;

    options.AddFixedWindowLimiter(policyName: "FixedWindowPolicy", opt =>
    {
        opt.PermitLimit = 5;       // Allow 5 requests
        opt.Window = TimeSpan.FromSeconds(10); // in a 10-second window
        opt.QueueProcessingOrder = System.Threading.RateLimiting.QueueProcessingOrder.OldestFirst;
        opt.QueueLimit = 2;
    });
});

// --- 6. REGISTER YOUR APPLICATION SERVICES ---
builder.Services.AddScoped<IUrlShortenerService, UrlShortenerService>();
builder.Services.AddValidatorsFromAssemblyContaining<IUrlShortenerService>();

// --- 7. SWAGGER (API DOCUMENTATION) ---
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// --- 8. AUTOMATIC DATABASE MIGRATION ---
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    await dbContext.Database.MigrateAsync();
}

// --- 9. CONFIGURE MIDDLEWARE ---
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(); // Corrected typo
}

app.UseHttpsRedirection();

// Serve default files like index.html
app.UseDefaultFiles();
// Serve all static files from the wwwroot folder
app.UseStaticFiles();

app.UseCors();

// These MUST be in the correct order
app.UseAuthentication();
app.UseAuthorization();
app.UseRateLimiter();

// --- 10. DEFINE API ENDPOINTS ---

// "CREATE LINK" API ENDPOINT (SECURED)
app.MapPost("/api/shorten",
    async (
        CreateShortUrlRequest request,
        IUrlShortenerService service,
        IValidator<CreateShortUrlRequest> validator,
        HttpContext httpContext,
        ClaimsPrincipal user // Added user
    ) =>
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        var response = await service.CreateShortUrlAsync(
            request, // Pass the full request
            httpContext.Request.Scheme,
            httpContext.Request.Host.ToString(),
            user // Pass the user
        );

        return Results.Ok(response);
    })
.RequireRateLimiting("FixedWindowPolicy")
.RequireAuthorization();

// "GET MY LINKS" ENDPOINT (SECURED)
app.MapGet("/api/my-links", async (
    ApplicationDbContext dbContext,
    ClaimsPrincipal user
) =>
{
    var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);

    if (string.IsNullOrEmpty(userId))
    {
        return Results.Unauthorized();
    }

    var links = await dbContext.ShortenedUrls
        .Where(s => s.UserId == userId)
        .OrderByDescending(s => s.CreatedOnUtc)
        .ToListAsync();

    return Results.Ok(links);
})
.RequireAuthorization();

// "IDENTITY" ENDPOINTS (e.g., /register, /login)
app.MapIdentityApi<IdentityUser>();

// "REDIRECT" ENDPOINT (PUBLIC)
app.MapGet("/{shortCode}", async (string shortCode, IUrlShortenerService service) =>
{
    var longUrl = await service.GetLongUrlAsync(shortCode);

    if (longUrl is null)
    {
        // If not found, fall through to the Vue app
        return Results.NotFound();
    }

    return Results.Redirect(longUrl, permanent: true);
});

app.MapFallbackToFile("index.html");

app.Run();