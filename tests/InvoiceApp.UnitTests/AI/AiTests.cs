using InvoiceApp.AI.Agents;
using InvoiceApp.AI.Common;
using InvoiceApp.AI.Coordinator;
using InvoiceApp.AI.Evaluations;
using InvoiceApp.AI.Memory;
using InvoiceApp.AI.Telemetry;
using InvoiceApp.AI.Tools;
using InvoiceApp.Application.Common.Messaging;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace InvoiceApp.UnitTests.AI;

public class AiTests
{
    private class FakeDispatcher : IDispatcher
    {
        public Task<TResult> SendAsync<TResult>(ICommand<TResult> command, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();

        public Task<TResult> QueryAsync<TResult>(IQuery<TResult> query, CancellationToken cancellationToken = default) =>
            throw new NotImplementedException();
    }

    [Fact]
    public void CustomerTools_GetTools_ReturnsExpectedTools()
    {
        var tools = new CustomerTools(new FakeDispatcher(), NullLogger<CustomerTools>.Instance);
        var aiTools = tools.GetTools().ToList();

        Assert.Equal(5, aiTools.Count);
        Assert.Contains(aiTools, t => t.Name == "CreateCustomer");
        Assert.Contains(aiTools, t => t.Name == "UpdateCustomer");
        Assert.Contains(aiTools, t => t.Name == "GetCustomerProfile");
        Assert.Contains(aiTools, t => t.Name == "FindCustomer");
        Assert.Contains(aiTools, t => t.Name == "ListCustomers");
    }

    [Fact]
    public void InvoiceTools_GetTools_ReturnsExpectedTools()
    {
        var tools = new InvoiceTools(new FakeDispatcher(), NullLogger<InvoiceTools>.Instance);
        var aiTools = tools.GetTools().ToList();

        Assert.Equal(7, aiTools.Count);
        Assert.Contains(aiTools, t => t.Name == "CreateInvoice");
        Assert.Contains(aiTools, t => t.Name == "GetInvoiceByNumber");
        Assert.Contains(aiTools, t => t.Name == "UpdateInvoiceStatus");
        Assert.Contains(aiTools, t => t.Name == "DeleteDraftInvoice");
        Assert.Contains(aiTools, t => t.Name == "GetOverdueInvoices");
        Assert.Contains(aiTools, t => t.Name == "GetInvoiceSummary");
        Assert.Contains(aiTools, t => t.Name == "ListInvoices");
    }

    [Fact]
    public void DocumentationTools_GetTools_ReturnsExpectedTools()
    {
        var tools = new DocumentationTools(NullLogger<DocumentationTools>.Instance);
        var aiTools = tools.GetTools().ToList();

        Assert.Single(aiTools);
        Assert.Equal("ReadDocumentationPage", aiTools[0].Name);
    }

    [Fact]
    public async Task DocumentationTools_ReadPage_ReturnsPageOrNotFound()
    {
        var tools = new DocumentationTools(NullLogger<DocumentationTools>.Instance);
        var result = await tools.ReadDocumentationPage("getting-started");

        Assert.NotNull(result);
        Assert.NotEmpty(result);
    }

    [Fact]
    public async Task AgentCoordinator_ProcessAsync_InSimulationMode_ExecutesAndRecordsMemory()
    {
        var fakeDispatcher = new FakeDispatcher();
        var simulationMode = new SimulationMode(true);
        var telemetry = new TelemetryService(NullLogger<TelemetryService>.Instance);
        var memory = new AgentMemoryService();
        var eval = new AgentEvaluationService(NullLogger<AgentEvaluationService>.Instance);
        var options = Options.Create(new AzureOpenAISettings());

        var customerTools = new CustomerTools(fakeDispatcher, NullLogger<CustomerTools>.Instance);
        var invoiceTools = new InvoiceTools(fakeDispatcher, NullLogger<InvoiceTools>.Instance);
        var docTools = new DocumentationTools(NullLogger<DocumentationTools>.Instance);

        var agents = new List<IBaseAgent>
        {
            new InvoiceAssistantAgent(null, invoiceTools, customerTools, telemetry, NullLogger<InvoiceAssistantAgent>.Instance, options, simulationMode),
            new CustomerOperationsAgent(null, customerTools, telemetry, NullLogger<CustomerOperationsAgent>.Instance, options, simulationMode),
            new AnalyticsAgent(null, invoiceTools, customerTools, telemetry, NullLogger<AnalyticsAgent>.Instance, options, simulationMode),
            new DocumentationAgent(null, docTools, telemetry, NullLogger<DocumentationAgent>.Instance, options, simulationMode)
        };

        var coordinator = new AgentCoordinator(agents, memory, eval, telemetry, NullLogger<AgentCoordinator>.Instance);

        var conversationId = "test-conv-1";
        var result = await coordinator.ProcessAsync(conversationId, "How do I create an invoice in InvoiceAI?");

        Assert.NotNull(result);
        Assert.True(result.AgentResponse.IsSuccess);
        Assert.Equal("Platform Documentation Agent", result.AgentResponse.AgentName);
        Assert.True(result.Evaluation.ConfidenceScore > 0);

        var history = await memory.GetConversationHistoryAsync(conversationId);
        Assert.Contains("How do I create an invoice", history);
        Assert.Contains("Platform Documentation Agent", history);
    }

    [Fact]
    public async Task AgentCoordinator_RoutesCustomerQuery_ToCustomerOperationsAgent()
    {
        var fakeDispatcher = new FakeDispatcher();
        var simulationMode = new SimulationMode(true);
        var telemetry = new TelemetryService(NullLogger<TelemetryService>.Instance);
        var memory = new AgentMemoryService();
        var eval = new AgentEvaluationService(NullLogger<AgentEvaluationService>.Instance);
        var options = Options.Create(new AzureOpenAISettings());

        var customerTools = new CustomerTools(fakeDispatcher, NullLogger<CustomerTools>.Instance);
        var invoiceTools = new InvoiceTools(fakeDispatcher, NullLogger<InvoiceTools>.Instance);
        var docTools = new DocumentationTools(NullLogger<DocumentationTools>.Instance);

        var agents = new List<IBaseAgent>
        {
            new InvoiceAssistantAgent(null, invoiceTools, customerTools, telemetry, NullLogger<InvoiceAssistantAgent>.Instance, options, simulationMode),
            new CustomerOperationsAgent(null, customerTools, telemetry, NullLogger<CustomerOperationsAgent>.Instance, options, simulationMode),
            new AnalyticsAgent(null, invoiceTools, customerTools, telemetry, NullLogger<AnalyticsAgent>.Instance, options, simulationMode),
            new DocumentationAgent(null, docTools, telemetry, NullLogger<DocumentationAgent>.Instance, options, simulationMode)
        };

        var coordinator = new AgentCoordinator(agents, memory, eval, telemetry, NullLogger<AgentCoordinator>.Instance);

        var result = await coordinator.ProcessAsync("test-conv-2", "Create customer Apex Global Ltd with email billing@apex.com");

        Assert.NotNull(result);
        Assert.True(result.AgentResponse.IsSuccess);
        Assert.Equal("Customer Operations Agent", result.AgentResponse.AgentName);
    }
}
