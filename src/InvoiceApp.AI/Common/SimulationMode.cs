namespace InvoiceApp.AI.Common;

/// <summary>
/// Value object representing whether the AI subsystem is currently operating in offline Simulation Mode.
/// </summary>
/// <remarks>
/// <para>
/// When enabled (<see cref="IsEnabled"/> is <c>true</c>), agents bypass external cloud calls to Azure OpenAI
/// and simulate intelligent responses while still performing full real-world in-process tool execution
/// (e.g. creating customers, creating invoices, transitioning statuses) against the local database.
/// </para>
/// <para>
/// This allows developers, continuous integration (CI) pipelines, and test suites to run completely offline
/// without requiring Azure subscriptions, API keys, or incurring cloud costs.
/// </para>
/// </remarks>
/// <param name="IsEnabled">Indicates whether simulation mode is currently active.</param>
public sealed record SimulationMode(bool IsEnabled);
