// This is the correct .NET 8.0-compatible code for Program.cs

var builder = WebApplication.CreateBuilder(args);

// 1. This is the .NET 8 way to add Swagger/OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// 2. This is the .NET 8 way to use Swagger/OpenAPI
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

// This is your "Hello World" endpoint. We'll replace this later.
app.MapGet("/", () => "Hello World! My pipeline works!");

app.Run();