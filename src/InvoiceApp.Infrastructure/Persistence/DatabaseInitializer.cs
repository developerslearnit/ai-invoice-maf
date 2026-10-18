using Dapper;
using InvoiceApp.Domain.Entities;
using InvoiceApp.Domain.Enums;
using InvoiceApp.Infrastructure.Connection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace InvoiceApp.Infrastructure.Persistence;

public sealed class DatabaseInitializer
{
    private readonly InvoiceAppDbContext _dbContext;
    private readonly IDbConnectionFactory _connectionFactory;
    private readonly ILogger<DatabaseInitializer> _logger;

    public DatabaseInitializer(
        InvoiceAppDbContext dbContext,
        IDbConnectionFactory connectionFactory,
        ILogger<DatabaseInitializer> logger)
    {
        _dbContext = dbContext;
        _connectionFactory = connectionFactory;
        _logger = logger;
    }

    public async Task InitializeAsync(bool forceReseed = false, CancellationToken cancellationToken = default)
    {
        try
        {
            _logger.LogInformation("Applying MSSQL database migrations...");
            await _dbContext.Database.MigrateAsync(cancellationToken);

            using var connection = await _connectionFactory.CreateOpenConnectionAsync(cancellationToken);
            await SeedDataAsync(connection, forceReseed, cancellationToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "An error occurred while initializing the database.");
            throw;
        }
    }

    private async Task SeedDataAsync(System.Data.Common.DbConnection connection, bool forceReseed, CancellationToken cancellationToken)
    {
        var customerCount = await _dbContext.Customers.CountAsync(cancellationToken);
        if (!forceReseed && customerCount >= 10)
        {
            return;
        }

        if (customerCount > 0)
        {
            _logger.LogInformation("Refreshing sample data to ensure 10 customers and 5 invoices each...");
            _dbContext.InvoiceItems.RemoveRange(_dbContext.InvoiceItems);
            _dbContext.Invoices.RemoveRange(_dbContext.Invoices);
            _dbContext.Customers.RemoveRange(_dbContext.Customers);
            await _dbContext.SaveChangesAsync(cancellationToken);
        }

        _logger.LogInformation("Seeding 10 customers and 5 invoices for each into SQL Server...");

        var today = DateOnly.FromDateTime(DateTime.UtcNow);

        // 1. Contoso Ltd
        var c1 = Customer.Create("Contoso Ltd", "billing@contoso.com", "+1-555-0100", "100 Microsoft Way, Redmond, WA").Value;
        // 2. Acme Corporation
        var c2 = Customer.Create("Acme Corporation", "finance@acme.com", "+1-555-0200", "742 Evergreen Terrace, Springfield").Value;
        // 3. Fabrikam Inc
        var c3 = Customer.Create("Fabrikam Inc", "accounts@fabrikam.com", "+1-555-0300", "123 Main St, Seattle, WA").Value;
        // 4. Northwind Traders
        var c4 = Customer.Create("Northwind Traders", "orders@northwind.com", "+44-20-7946-0912", "14 Bishopsgate, London, UK").Value;
        // 5. AdventureWorks Ltd
        var c5 = Customer.Create("AdventureWorks Ltd", "invoicing@adventureworks.com", "+61-2-9876-5432", "42 George St, Sydney, Australia").Value;
        // 6. Flutterwave Tech
        var c6 = Customer.Create("Flutterwave Tech", "payments@flutterwave.com", "+234-1-888-0101", "8 Providence St, Lekki, Lagos").Value;
        // 7. Interswitch Group
        var c7 = Customer.Create("Interswitch Group", "billing@interswitch.com", "+234-1-888-0202", "1644 Oko Awo St, Victoria Island, Lagos").Value;
        // 8. Paystack Payments
        var c8 = Customer.Create("Paystack Payments", "finance@paystack.com", "+234-1-888-0303", "126 Joel Ogunnaike St, Ikeja, Lagos").Value;
        // 9. Dangote Industries
        var c9 = Customer.Create("Dangote Industries", "procurement@dangote-group.com", "+234-1-888-0404", "1 Alfred Rewane Rd, Ikoyi, Lagos").Value;
        // 10. Globacom Telecoms
        var c10 = Customer.Create("Globacom Telecoms", "enterprise@gloworld.com", "+234-1-888-0505", "Mike Adenuga Towers, Victoria Island, Lagos").Value;

        var customers = new[] { c1, c2, c3, c4, c5, c6, c7, c8, c9, c10 };
        await _dbContext.Customers.AddRangeAsync(customers, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        var invoices = new List<Invoice>();
        int invoiceSeq = 1;

        // Helper to create and add invoice
        void AddInvoice(Customer customer, InvoiceStatus status, int daysAgoIssue, int dueInDays, string notes, (string Desc, decimal Qty, decimal Price)[] items)
        {
            var number = $"INV-{invoiceSeq:D6}";
            invoiceSeq++;

            var issueDate = today.AddDays(-daysAgoIssue);
            var dueDate = today.AddDays(dueInDays);

            var inv = Invoice.Create(number, customer.Id, issueDate, dueDate, notes, items).Value;

            switch (status)
            {
                case InvoiceStatus.Sent:
                    inv.TransitionTo(InvoiceStatus.Sent);
                    break;
                case InvoiceStatus.Paid:
                    inv.TransitionTo(InvoiceStatus.Sent);
                    inv.TransitionTo(InvoiceStatus.Paid);
                    break;
                case InvoiceStatus.Overdue:
                    inv.TransitionTo(InvoiceStatus.Sent);
                    inv.TransitionTo(InvoiceStatus.Overdue);
                    break;
                case InvoiceStatus.Cancelled:
                    inv.TransitionTo(InvoiceStatus.Cancelled);
                    break;
                case InvoiceStatus.Draft:
                default:
                    // remains draft
                    break;
            }

            invoices.Add(inv);
        }

        // --- Customer 1: Contoso Ltd (5 Invoices) ---
        AddInvoice(c1, InvoiceStatus.Draft, 2, 28, "Cloud infrastructure advisory",
            [("Cloud Architecture Workshop", 2, 1500000m), ("DevOps CI/CD Pipeline Setup", 1, 2000000m)]);
        AddInvoice(c1, InvoiceStatus.Sent, 12, 18, "Enterprise software licensing and support",
            [("Enterprise Platform License", 5, 500000m), ("Priority Support Retainer", 1, 1000000m)]);
        AddInvoice(c1, InvoiceStatus.Paid, 45, -15, "Initial technical discovery & blueprinting",
            [("Technical Discovery Phase", 1, 3500000m), ("Solution Architecture Blueprint", 1, 2500000m)]);
        AddInvoice(c1, InvoiceStatus.Overdue, 65, -20, "Q3 Modernization training",
            [("ASP.NET Core Modernization Training", 3, 1200000m), ("Course Lab Environments", 10, 50000m)]);
        AddInvoice(c1, InvoiceStatus.Cancelled, 20, 10, "Prototype add-on (cancelled per customer request)",
            [("Legacy Sync Plugin Prototype", 1, 800000m)]);

        // --- Customer 2: Acme Corporation (5 Invoices) ---
        AddInvoice(c2, InvoiceStatus.Draft, 1, 29, "Infrastructure automation",
            [("Terraform Cloud Setup", 2, 1200000m), ("Kubernetes Cluster Hardening", 1, 2800000m)]);
        AddInvoice(c2, InvoiceStatus.Sent, 10, 20, "Custom API integration endpoints",
            [("Webhook Streaming Connector", 1, 2200000m), ("API Gateway Policy Configuration", 1, 1400000m)]);
        AddInvoice(c2, InvoiceStatus.Paid, 35, -5, "Security audit & static analysis",
            [("Static Code Security Analysis", 1, 1800000m), ("Penetration Testing Review", 1, 2200000m)]);
        AddInvoice(c2, InvoiceStatus.Overdue, 55, -10, "Annual maintenance contract 2026",
            [("24/7 SLA Maintenance Tier", 1, 4500000m)]);
        AddInvoice(c2, InvoiceStatus.Paid, 80, -30, "Database performance optimization",
            [("SQL Query Index Profiling", 1, 1600000m), ("Cache Layer Architecture", 1, 1900000m)]);

        // --- Customer 3: Fabrikam Inc (5 Invoices) ---
        AddInvoice(c3, InvoiceStatus.Draft, 3, 27, "Design system implementation",
            [("Tailwind Design Tokens Setup", 1, 1100000m), ("Reusable UI Component Kit", 1, 2400000m)]);
        AddInvoice(c3, InvoiceStatus.Sent, 14, 16, "Data pipeline architecture",
            [("ETL Event Hub Ingestion", 1, 3200000m), ("Real-time Analytics Dashboard", 1, 1800000m)]);
        AddInvoice(c3, InvoiceStatus.Paid, 50, -10, "Managed cloud hosting Q1",
            [("Cloud App Service Hosting", 3, 900000m), ("Traffic Manager Load Balancer", 3, 300000m)]);
        AddInvoice(c3, InvoiceStatus.Overdue, 70, -25, "Disaster recovery failover setup",
            [("Geo-replication Configuration", 1, 2700000m), ("DR Drill Simulation", 1, 1500000m)]);
        AddInvoice(c3, InvoiceStatus.Sent, 5, 25, "AI readiness assessment",
            [("AI Agent Framework Evaluation", 1, 3000000m)]);

        // --- Customer 4: Northwind Traders (5 Invoices) ---
        AddInvoice(c4, InvoiceStatus.Paid, 40, -10, "Supply chain analytics portal",
            [("Supply Chain Dashboard", 1, 4200000m), ("Supplier Integration Service", 1, 1800000m)]);
        AddInvoice(c4, InvoiceStatus.Sent, 8, 22, "Inventory management system integration",
            [("Warehouse Barcode API", 1, 2500000m), ("Stock Level Webhook Stream", 1, 1200000m)]);
        AddInvoice(c4, InvoiceStatus.Draft, 2, 28, "Firmware update management",
            [("IoT Gateway Firmware Validation", 1, 1900000m)]);
        AddInvoice(c4, InvoiceStatus.Overdue, 60, -15, "Logistics fleet telemetry service",
            [("GPS Fleet Tracking Server", 1, 3800000m), ("Driver Mobile App Backing", 1, 2200000m)]);
        AddInvoice(c4, InvoiceStatus.Paid, 90, -45, "Multi-currency checkout module",
            [("Naira & Forex Payment Gateway", 1, 3100000m), ("Automated FX Converter", 1, 900000m)]);

        // --- Customer 5: AdventureWorks Ltd (5 Invoices) ---
        AddInvoice(c5, InvoiceStatus.Sent, 15, 15, "Global e-commerce portal",
            [("B2B Catalog Engine", 1, 4500000m), ("Search & Filter Elastic Service", 1, 2500000m)]);
        AddInvoice(c5, InvoiceStatus.Paid, 42, -12, "Mobile application API integration",
            [("GraphQL Mobile Endpoint Layer", 1, 3600000m), ("Push Notification Dispatcher", 1, 1400000m)]);
        AddInvoice(c5, InvoiceStatus.Draft, 4, 26, "Customer loyalty rewards program",
            [("Loyalty Points Engine", 1, 2100000m), ("Tiered Discount Rules", 1, 1200000m)]);
        AddInvoice(c5, InvoiceStatus.Overdue, 75, -30, "Point of Sale retail hardware integration",
            [("POS Fiscal Device Driver", 2, 1750000m), ("Store Synchronization Agent", 1, 1500000m)]);
        AddInvoice(c5, InvoiceStatus.Cancelled, 25, 5, "Retired legacy ERP sync service",
            [("AS400 Connector Protocol", 1, 1200000m)]);

        // --- Customer 6: Flutterwave Tech (5 Invoices) ---
        AddInvoice(c6, InvoiceStatus.Paid, 38, -8, "Core payment gateway architecture",
            [("High-Speed Settlement Router", 1, 5500000m), ("Card Vault Tokenizer", 1, 3500000m)]);
        AddInvoice(c6, InvoiceStatus.Sent, 11, 19, "PCI-DSS compliance remediation",
            [("Network Segmentation Review", 1, 2400000m), ("Encryption at Rest Audit", 1, 2100000m)]);
        AddInvoice(c6, InvoiceStatus.Overdue, 52, -12, "Merchant settlement batch processor",
            [("Instant Payouts Engine", 1, 4200000m), ("NIP Interbank Bridge", 1, 2800000m)]);
        AddInvoice(c6, InvoiceStatus.Draft, 3, 27, "Virtual card issuing API module",
            [("Mastercard / Visa Card Generator", 1, 3800000m)]);
        AddInvoice(c6, InvoiceStatus.Paid, 85, -40, "Real-time transaction fraud guard",
            [("Machine Learning Scoring Service", 1, 4800000m), ("Velocity Rules Engine", 1, 2200000m)]);

        // --- Customer 7: Interswitch Group (5 Invoices) ---
        AddInvoice(c7, InvoiceStatus.Paid, 44, -14, "National switching node enhancement",
            [("ISO 8583 Message Parser", 1, 6000000m), ("Switching Fault Tolerance", 1, 3000000m)]);
        AddInvoice(c7, InvoiceStatus.Sent, 9, 21, "ATM terminal management system",
            [("NDC+ Protocol Driver", 1, 2900000m), ("Remote Key Injection Protocol", 1, 1800000m)]);
        AddInvoice(c7, InvoiceStatus.Overdue, 64, -18, "Tokenized mobile payments upgrade",
            [("NFC Host Card Emulation", 1, 3700000m), ("Secure Element Connector", 1, 2300000m)]);
        AddInvoice(c7, InvoiceStatus.Draft, 2, 28, "Cross-border remittance corridor",
            [("Pan-African Payment Settlement", 1, 4500000m)]);
        AddInvoice(c7, InvoiceStatus.Paid, 95, -50, "DevSecOps container pipeline hardening",
            [("Docker Image Vulnerability Scanning", 1, 1800000m), ("Kubernetes RBAC Hardening", 1, 2200000m)]);

        // --- Customer 8: Paystack Payments (5 Invoices) ---
        AddInvoice(c8, InvoiceStatus.Sent, 16, 14, "Automated recurring subscriptions platform",
            [("Billing Cycle Scheduler", 1, 2800000m), ("Dunning Email & Retry Logic", 1, 1700000m)]);
        AddInvoice(c8, InvoiceStatus.Paid, 36, -6, "Developer documentation & API playground",
            [("Interactive OpenAPI Console", 1, 2100000m), ("SDK Code Sample Generator", 1, 1400000m)]);
        AddInvoice(c8, InvoiceStatus.Draft, 5, 25, "Direct debit authorization workflow",
            [("NIBSS Mandate Engine", 1, 3200000m)]);
        AddInvoice(c8, InvoiceStatus.Overdue, 58, -16, "Customer identity verification (BVN/NIN)",
            [("Biometric Verification Adapter", 1, 3400000m), ("Government Registry Lookup", 1, 2100000m)]);
        AddInvoice(c8, InvoiceStatus.Paid, 88, -42, "High-availability webhook dispatcher",
            [("Kafka Webhook Queueing Layer", 1, 3600000m), ("Backoff Retries Cluster", 1, 1900000m)]);

        // --- Customer 9: Dangote Industries (5 Invoices) ---
        AddInvoice(c9, InvoiceStatus.Paid, 48, -18, "Enterprise ERP data synchronization",
            [("SAP S/4HANA Middleware", 1, 7500000m), ("Master Data Management", 1, 3500000m)]);
        AddInvoice(c9, InvoiceStatus.Sent, 13, 17, "Cement plant supply chain tracking",
            [("Truck Dispatching Scheduler", 1, 4100000m), ("Weighbridge Automation Interface", 1, 2400000m)]);
        AddInvoice(c9, InvoiceStatus.Overdue, 68, -22, "Factory IoT predictive maintenance sensors",
            [("Vibration Sensor Telemetry", 1, 3900000m), ("Thermal Anomaly Detector", 1, 2600000m)]);
        AddInvoice(c9, InvoiceStatus.Draft, 4, 26, "Central refinery warehouse inventory system",
            [("Raw Material Barcoding Module", 1, 3200000m)]);
        AddInvoice(c9, InvoiceStatus.Paid, 100, -55, "Fleet telematics and fuel consumption system",
            [("OBD-II Telematics Tracker", 100, 45000m), ("Fuel Consumption Analytics", 1, 2000000m)]);

        // --- Customer 10: Globacom Telecoms (5 Invoices) ---
        AddInvoice(c10, InvoiceStatus.Sent, 12, 18, "5G Core network performance analytics",
            [("Cell Tower Throughput Monitor", 1, 4800000m), ("QoS Traffic Analyzer", 1, 2700000m)]);
        AddInvoice(c10, InvoiceStatus.Paid, 39, -9, "Self-service subscriber mobile application",
            [("Self-Care Account App Backend", 1, 3400000m), ("Airtime / Data Recharger Gateway", 1, 1800000m)]);
        AddInvoice(c10, InvoiceStatus.Overdue, 56, -14, "Value-Added Services (VAS) billing engine",
            [("Caller Tune Subscription Manager", 1, 2900000m), ("SMS Shortcode Gateway", 1, 1900000m)]);
        AddInvoice(c10, InvoiceStatus.Draft, 1, 29, "Billing mediation cluster upgrade",
            [("Call Detail Record (CDR) Ingestion", 1, 3700000m), ("Rating & Charging Engine", 1, 2800000m)]);
        AddInvoice(c10, InvoiceStatus.Cancelled, 30, 0, "Retired USSD interactive gateway module",
            [("Legacy USSD SMPP Connector", 1, 1500000m)]);

        await _dbContext.Invoices.AddRangeAsync(invoices, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);

        // Synchronize sequence past 50
        await connection.ExecuteAsync(new CommandDefinition(
            "ALTER SEQUENCE invoice_number_seq RESTART WITH 51;",
            cancellationToken: cancellationToken));

        _logger.LogInformation("Database seeded successfully with exactly 10 customers and 50 invoices (5 each) in SQL Server.");
    }
}
