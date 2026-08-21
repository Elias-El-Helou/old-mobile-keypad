using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using OldMobileKeypad.Api;

var builder = WebApplication.CreateBuilder(args);

// ============================================================================
// 1. ADD SERVICES TO THE CONTAINER
// ============================================================================

// Add CORS support
// This allows customers to call the API from web browsers (different domains)
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAll", policy =>
    {
        policy.AllowAnyOrigin()      // Allow any domain to call this API
              .AllowAnyMethod()       // Allow GET, POST, PUT, DELETE, etc.
              .AllowAnyHeader();      // Allow any headers
    });
});

// Add API Explorer and Swagger documentation
// This generates interactive API documentation
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
    {
        Title = "OldPhonePad Decoder API",
        Version = "1.0.0",
        Description = "A REST API for decoding old phone keypad (T9) sequences into readable text.",
        Contact = new Microsoft.OpenApi.Models.OpenApiContact
        {
            Name = "Software Challenge"
            // Url = new Uri("https://www.ironsoftware.com")
        }
    });
});

// ============================================================================
// 2. BUILD THE APPLICATION
// ============================================================================
var app = builder.Build();

// ============================================================================
// 3. CONFIGURE THE HTTP REQUEST PIPELINE
// ============================================================================

// Enable Swagger UI (interactive API documentation)
// In development, Swagger is automatically enabled
// In production, you might want to disable it for security
// if (app.Environment.IsDevelopment())
// {
//     app.UseSwagger();
//     app.UseSwaggerUI(options =>
//     {
//         options.SwaggerEndpoint("/swagger/v1/swagger.json", "OldPhonePad API v1");
//         options.RoutePrefix = string.Empty;  // Serve Swagger UI at root (http://localhost:5000)
//     });
// }

Console.WriteLine($"Environment: {app.Environment.EnvironmentName}");
Console.WriteLine($"IsDevelopment: {app.Environment.IsDevelopment()}");

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/swagger/v1/swagger.json", "OldPhonePad API v1");
        options.RoutePrefix = string.Empty;
    });
}
else
{
    Console.WriteLine("⚠️  WARNING: Not in Development mode - Swagger not registered!");
}

// Enable HTTPS redirection (in production)
app.UseHttpsRedirection();

// Enable CORS
app.UseCors("AllowAll");

// ============================================================================
// 4. MAP API ENDPOINTS
// ============================================================================

// Register all OldPhonePad endpoints
app.MapOldPhonePadApi();

// Root endpoint - redirect to Swagger UI
// app.MapGet("/", () => Results.Redirect("/swagger"))
//     .ExcludeFromDescription();  // Don't show this in Swagger docs
app.MapGet("/api/oldphonepad", () => Results.Redirect("/"))
    .ExcludeFromDescription();
// ============================================================================
// 5. RUN THE APPLICATION
// ============================================================================
app.Run();