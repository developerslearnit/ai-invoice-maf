using System.Text.Json.Serialization;
using InvoiceApp.AI;
using InvoiceApp.Api.Endpoints;
using InvoiceApp.Api.Middleware;
using InvoiceApp.Application;
using InvoiceApp.Infrastructure;
using InvoiceApp.Infrastructure.Persistence;
using Microsoft.OpenApi;

var builder = WebApplication.CreateBuilder(args);

builder.Services.ConfigureHttpJsonOptions(options =>
{
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter());
    options.SerializerOptions.DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull;
});

builder.Services.AddProblemDetails();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Invoice Management REST API",
        Version = "v1",
        Description = "A clean ASP.NET Core Invoice Management API designed for Microsoft Agent Framework training."
    });
});

builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);
builder.Services.AddAgentFramework(builder.Configuration);

var app = builder.Build();

app.UseMiddleware<GlobalExceptionHandlingMiddleware>();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI(c =>
    {
        c.SwaggerEndpoint("/swagger/v1/swagger.json", "Invoice Management API v1");
        c.RoutePrefix = string.Empty; // Serve Swagger UI at application root
    });
}

app.UseHttpsRedirection();

app.MapCustomerEndpoints();
app.MapInvoiceEndpoints();
app.MapAiEndpoints();

// Auto-initialize schema & seed data if database is reachable
using (var scope = app.Services.CreateScope())
{
    var initializer = scope.ServiceProvider.GetRequiredService<DatabaseInitializer>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    try
    {
        await initializer.InitializeAsync();
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Could not initialize database automatically at startup. Ensure SQL Server is accessible.");
    }
}

app.Run();

public partial class Program
{
    protected Program() { }
}
