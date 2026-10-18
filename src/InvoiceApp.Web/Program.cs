using System.Net.Http.Headers;
using InvoiceApp.Web.Services.Ai;
using InvoiceApp.Web.Services.ApiClients;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNameCaseInsensitive = true;
    });

builder.Services.AddAntiforgery(options =>
{
    options.HeaderName = "X-CSRF-TOKEN";
});

// Configure backend API base URL
var apiBaseUrl = builder.Configuration.GetValue<string>("ApiSettings:BaseUrl")!;
var apiUri = new Uri(apiBaseUrl.TrimEnd('/') + "/");

// Register Typed API Clients
builder.Services.AddHttpClient<ICustomerApiClient, CustomerApiClient>(client =>
{
    client.BaseAddress = apiUri;
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IInvoiceApiClient, InvoiceApiClient>(client =>
{
    client.BaseAddress = apiUri;
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IDashboardApiClient, DashboardApiClient>(client =>
{
    client.BaseAddress = apiUri;
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromSeconds(30);
});

builder.Services.AddHttpClient<IAiApiClient, AiApiClient>(client =>
{
    client.BaseAddress = apiUri;
    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    client.Timeout = TimeSpan.FromSeconds(60);
});

// Register AI Assistant services (Live Microsoft Agent Framework Coordinator with local simulation fallback)
builder.Services.AddScoped<MockAiAssistantService>();
builder.Services.AddScoped<IAiAssistantService, AgentFrameworkAiAssistantService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Dashboard/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAntiforgery();

app.MapControllers();

app.Run();
