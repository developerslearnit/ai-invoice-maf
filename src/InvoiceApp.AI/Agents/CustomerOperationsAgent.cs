using InvoiceApp.AI.Common;
using InvoiceApp.AI.Telemetry;
using InvoiceApp.AI.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InvoiceApp.AI.Agents;

/// <summary>
/// Specialized autonomous agent dedicated to customer relationship management, client onboarding,
/// contact information maintenance, and lifetime value analytics.
/// </summary>
/// <remarks>
/// <para>
/// Integrates domain tools from <see cref="CustomerTools"/> via Microsoft Agent Framework.
/// Enforces key customer business rules:
/// </para>
/// <list type="bullet">
///   <item><description>Customer email addresses must be unique across the entire platform.</description></item>
///   <item><description>Customers with linked invoices cannot be arbitrarily removed from the system.</description></item>
///   <item><description>Tracks debtor balances and payment reliability across multiple invoice lifecycles.</description></item>
/// </list>
/// </remarks>
public sealed class CustomerOperationsAgent : BaseAgent
{
    private readonly CustomerTools _customerTools;

    /// <summary>
    /// Initializes a new instance of <see cref="CustomerOperationsAgent"/>.
    /// </summary>
    /// <param name="chatClient">The underlying chat client instance.</param>
    /// <param name="customerTools">The customer domain tools provider.</param>
    /// <param name="telemetry">The telemetry service for execution tracking.</param>
    /// <param name="logger">The logger instance.</param>
    /// <param name="options">The Azure OpenAI configuration options.</param>
    /// <param name="simulationMode">The simulation mode toggle.</param>
    public CustomerOperationsAgent(
        IChatClient? chatClient,
        CustomerTools customerTools,
        ITelemetryService telemetry,
        ILogger<CustomerOperationsAgent> logger,
        IOptions<AzureOpenAISettings> options,
        SimulationMode simulationMode)
        : base(
            chatClient,
            instructions:
            """
            You are the specialized Customer Operations Agent for InvoiceAI.
            Your responsibilities include:
            1. Registering new customer profiles using CreateCustomer (Name and unique Email are required).
            2. Updating customer contact info, phone, and billing address using UpdateCustomer.
            3. Retrieving customer financial history, lifetime invoiced amounts, total paid, and outstanding balances using GetCustomerProfile.
            4. Searching for clients by name or email using FindCustomer.
            5. Enforcing customer business rules: customer emails must be unique, and customers with active invoices cannot be deleted.
            """,
            name: "Customer Operations Agent",
            description: "Onboards new customers, manages contact info, searches directory, and aggregates client lifetime values.",
            tools: customerTools.GetTools(),
            telemetry: telemetry,
            logger: logger,
            simulationMode: simulationMode)
    {
        _customerTools = customerTools;
    }

    /// <inheritdoc />
    protected override async Task<string> GenerateSimulatedResponseAsync(AgentRequest request, CancellationToken cancellationToken)
    {
        var lower = request.Prompt.ToLowerInvariant();

        if (lower.Contains("create customer") || lower.Contains("new customer") || lower.Contains("add customer"))
        {
            return "🏢 [SIMULATION]: Ready to onboard a customer. Bound tool `CreateCustomer` accepts: Name, Email (must be unique), Phone, and Address. For example: *'Create customer Apex Corp, email info@apex.com'*.";
        }

        if (lower.Contains("customer") || lower.Contains("profile") || lower.Contains("balance"))
        {
            var listJson = await _customerTools.ListCustomers(cancellationToken);
            return $"🏢 [SIMULATION]: Customer directory summary:\n\n{listJson}";
        }

        return await base.GenerateSimulatedResponseAsync(request, cancellationToken);
    }
}
