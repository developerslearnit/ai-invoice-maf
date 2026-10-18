using Azure.AI.OpenAI;
using InvoiceApp.AI.Agents;
using InvoiceApp.AI.Common;
using InvoiceApp.AI.Coordinator;
using InvoiceApp.AI.Evaluations;
using InvoiceApp.AI.Memory;
using InvoiceApp.AI.Telemetry;
using InvoiceApp.AI.Tools;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace InvoiceApp.AI;

/// <summary>
/// Provides extension methods for registering Microsoft Agent Framework, Azure OpenAI clients,
/// specialized autonomous agents, domain tools, memory, evaluation, and telemetry services into the dependency injection container.
/// </summary>
public static class DependencyInjection
{
    /// <summary>
    /// Registers all Microsoft Agent Framework AI components, specialized agents, domain tools, and coordinators.
    /// </summary>
    /// <param name="services">The service collection to populate.</param>
    /// <param name="configuration">The application configuration containing the "AzureOpenAI" section.</param>
    /// <returns>The modified <see cref="IServiceCollection"/> for method chaining.</returns>
    /// <remarks>
    /// <para>
    /// Execution stages performed during registration:
    /// </para>
    /// <list type="number">
    ///   <item><description>Reads <see cref="AzureOpenAISettings"/> from configuration.</description></item>
    ///   <item><description>Evaluates credentials to determine whether <see cref="SimulationMode"/> should be engaged.</description></item>
    ///   <item><description>Wires up <see cref="AzureOpenAIClient"/> and creates <see cref="IChatClient"/> with function invocation enabled (<c>.UseFunctionInvocation()</c>).</description></item>
    ///   <item><description>Registers telemetry (<see cref="ITelemetryService"/>), memory (<see cref="IAgentMemoryService"/>), and response evaluations (<see cref="IAgentEvaluationService"/>).</description></item>
    ///   <item><description>Registers domain tool providers (<see cref="CustomerTools"/>, <see cref="InvoiceTools"/>, <see cref="DocumentationTools"/>).</description></item>
    ///   <item><description>Registers the 4 specialized autonomous agents (<see cref="InvoiceAssistantAgent"/>, <see cref="CustomerOperationsAgent"/>, <see cref="AnalyticsAgent"/>, <see cref="DocumentationAgent"/>).</description></item>
    ///   <item><description>Registers the central orchestrator coordinator (<see cref="IAgentCoordinator"/> / <see cref="AgentCoordinator"/>).</description></item>
    /// </list>
    /// </remarks>
    public static IServiceCollection AddAgentFramework(this IServiceCollection services, IConfiguration configuration)
    {
        // 1. Configure settings manually to avoid extra configuration binding dependency issues
        var endpoint = configuration["AzureOpenAI:Endpoint"] ?? string.Empty;
        var apiKey = configuration["AzureOpenAI:ApiKey"] ?? string.Empty;
        var deploymentName = configuration["AzureOpenAI:DeploymentName"] ?? "gpt-4o";
        var embeddingDeploymentName = configuration["AzureOpenAI:EmbeddingDeploymentName"] ?? "text-embedding-3-small";

        var settings = new AzureOpenAISettings
        {
            Endpoint = endpoint,
            ApiKey = apiKey,
            DeploymentName = deploymentName,
            EmbeddingDeploymentName = embeddingDeploymentName
        };

        services.AddSingleton(Options.Create(settings));

        // 2. Determine simulation mode (offline fallback when keys/endpoints are missing or placeholders)
        bool isSimulated = string.IsNullOrWhiteSpace(apiKey) ||
                           apiKey.Contains("YOUR_AZURE_OPENAI_API_KEY", StringComparison.OrdinalIgnoreCase) ||
                           string.IsNullOrWhiteSpace(endpoint) ||
                           endpoint.Contains("your-resource", StringComparison.OrdinalIgnoreCase) ||
                           endpoint.Contains("your-openai-resource", StringComparison.OrdinalIgnoreCase);

        var simulationMode = new SimulationMode(isSimulated);
        services.AddSingleton(simulationMode);

        // 3. Register IChatClient (AzureOpenAIClient / Microsoft.Extensions.AI)
        if (isSimulated)
        {
            services.AddSingleton<IChatClient>(sp => null!);
        }
        else
        {
            services.AddSingleton<AzureOpenAIClient>(sp =>
            {
                return new AzureOpenAIClient(new Uri(endpoint), new System.ClientModel.ApiKeyCredential(apiKey));
            });

            services.AddSingleton<IChatClient>(sp =>
            {
                var client = sp.GetRequiredService<AzureOpenAIClient>();
                return client.GetChatClient(deploymentName)
                    .AsIChatClient()
                    .AsBuilder()
                    .UseFunctionInvocation()
                    .Build();
            });
        }

        // 4. Register Infrastructure, Telemetry & Memory services
        services.AddSingleton<ITelemetryService, TelemetryService>();
        services.AddSingleton<IAgentMemoryService, AgentMemoryService>();
        services.AddScoped<IAgentEvaluationService, AgentEvaluationService>();

        // 5. Register Domain & Platform Tools
        services.AddScoped<CustomerTools>();
        services.AddScoped<InvoiceTools>();
        services.AddScoped<DocumentationTools>();

        // 6. Register Specialized Autonomous Agents (IBaseAgent)
        services.AddScoped<IBaseAgent, InvoiceAssistantAgent>(sp => new InvoiceAssistantAgent(
            sp.GetService<IChatClient>(),
            sp.GetRequiredService<InvoiceTools>(),
            sp.GetRequiredService<CustomerTools>(),
            sp.GetRequiredService<ITelemetryService>(),
            sp.GetRequiredService<ILogger<InvoiceAssistantAgent>>(),
            sp.GetRequiredService<IOptions<AzureOpenAISettings>>(),
            sp.GetRequiredService<SimulationMode>()
        ));

        services.AddScoped<IBaseAgent, CustomerOperationsAgent>(sp => new CustomerOperationsAgent(
            sp.GetService<IChatClient>(),
            sp.GetRequiredService<CustomerTools>(),
            sp.GetRequiredService<ITelemetryService>(),
            sp.GetRequiredService<ILogger<CustomerOperationsAgent>>(),
            sp.GetRequiredService<IOptions<AzureOpenAISettings>>(),
            sp.GetRequiredService<SimulationMode>()
        ));

        services.AddScoped<IBaseAgent, AnalyticsAgent>(sp => new AnalyticsAgent(
            sp.GetService<IChatClient>(),
            sp.GetRequiredService<InvoiceTools>(),
            sp.GetRequiredService<CustomerTools>(),
            sp.GetRequiredService<ITelemetryService>(),
            sp.GetRequiredService<ILogger<AnalyticsAgent>>(),
            sp.GetRequiredService<IOptions<AzureOpenAISettings>>(),
            sp.GetRequiredService<SimulationMode>()
        ));

        services.AddScoped<IBaseAgent, DocumentationAgent>(sp => new DocumentationAgent(
            sp.GetService<IChatClient>(),
            sp.GetRequiredService<DocumentationTools>(),
            sp.GetRequiredService<ITelemetryService>(),
            sp.GetRequiredService<ILogger<DocumentationAgent>>(),
            sp.GetRequiredService<IOptions<AzureOpenAISettings>>(),
            sp.GetRequiredService<SimulationMode>()
        ));

        // 7. Register Orchestrator Coordinator
        services.AddScoped<IAgentCoordinator, AgentCoordinator>();
        services.AddScoped<AgentCoordinator>();

        return services;
    }
}
