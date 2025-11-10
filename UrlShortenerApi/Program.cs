using FluentValidation;
using Microsoft.AspNetCore.Identity; // For Identity
using Microsoft.AspNetCore.RateLimiting; // For Rate Limiting
using Microsoft.EntityFrameworkCore;
using UrlShortener.Application;
using UrlShortener.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

// --- 1. DEFINE  CORS POLICY ---
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

// --- 6. REGISTER APPLICATION SERVICES ---
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
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// --- 9. CONFIGURE MIDDLEWARE ---

// This is Vue.js app's static files (e.g., index.html)
app.UseDefaultFiles();
app.UseStaticFiles();

app.UseCors();


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
        System.Security.Claims.ClaimsPrincipal user // <-- 1. GET THE USER
    ) =>
    {
        var validationResult = await validator.ValidateAsync(request);
        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        // 2. PASS THE USER TO THE SERVICE
        var response = await service.CreateShortUrlAsync(
            request,
            httpContext.Request.Scheme,
            httpContext.Request.Host.ToString(),
            user // <-- 3. PASS THE USER
        );

        return Results.Ok(response);
    })
.RequireRateLimiting("FixedWindowPolicy")
.RequireAuthorization();

// --- THIS IS THE NEW "MY LINKS" ENDPOINT ---
app.MapGet("/api/my-links", async (
    IUrlShortenerService service,
    ApplicationDbContext dbContext,
    System.Security.Claims.ClaimsPrincipal user
) =>
{
    var userId = user.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);

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


app.MapFallbackToFile("index.html");

app.Run();