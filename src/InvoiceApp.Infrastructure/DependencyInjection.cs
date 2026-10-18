using InvoiceApp.Domain.Repositories;
using InvoiceApp.Domain.Services;
using InvoiceApp.Domain.UnitOfWork;
using InvoiceApp.Infrastructure.Connection;
using InvoiceApp.Infrastructure.Persistence;
using InvoiceApp.Infrastructure.Persistence.Repositories;
using InvoiceApp.Infrastructure.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace InvoiceApp.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection")
            ?? configuration["Database:ConnectionString"]
            ?? "Server=(localdb)\\mssqllocaldb;Database=InvoiceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True";

        services.AddDbContext<InvoiceAppDbContext>(options =>
        {
            options.UseSqlServer(connectionString);
        });

        services.AddSingleton<IDbConnectionFactory>(_ => new SqlConnectionFactory(connectionString));

        services.AddScoped<ICustomerRepository, CustomerRepository>();
        services.AddScoped<IInvoiceRepository, InvoiceRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        services.AddScoped<ICustomerReadRepository, CustomerReadRepository>();
        services.AddScoped<IInvoiceReadRepository, InvoiceReadRepository>();

        services.AddScoped<IInvoiceNumberGenerator, SqlServerInvoiceNumberGenerator>();
        services.AddScoped<DatabaseInitializer>();

        return services;
    }
}
