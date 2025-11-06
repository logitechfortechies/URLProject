using FluentValidation;
using Microsoft.AspNetCore.Identity; // For Identity
using Microsoft.AspNetCore.RateLimiting; // For Rate Limiting
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application;
using UrlShortener.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- 1. DEFINE YOUR CORS POLICY ---
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.WithOrigins("http://localhost:5173")
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
        opt.PermitLimit = 5;
        opt.Window = TimeSpan.FromSeconds(10);
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

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwagggerUI();
}

app.UseHttpsRedirection();

// --- 9. CONFIGURE MIDDLEWARE ---

// This serves your Vue.js app's static files (e.g., index.html)
app.UseDefaultFiles();
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
        HttpContext httpContext
    ) =>
    {
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

        return Results.Ok(response);
    })
.RequireRateLimiting("FixedWindowPolicy") // Applies rate limit
.RequireAuthorization();                 // Applies Identity security

// "REDIRECT" ENDPOINT (PUBLIC)
app.MapGet("/{shortCode}", async (string shortCode, IUrlShortenerService service) =>
{
    var longUrl = await service.GetLongUrlAsync(shortCode);

    if (longUrl is null)
    {
        return Results.NotFound();
    }

    return Results.Redirect(longUrl, permanent: true);
});

// "IDENTITY" ENDPOINTS (e.g., /register, /login)
app.MapIdentityApi<IdentityUser>();

// --- THIS IS THE FIX for serving the VUE APP ---
// This must be LAST. It sends all other requests to your Vue app.
app.MapFallbackToFile("index.html");

app.Run();