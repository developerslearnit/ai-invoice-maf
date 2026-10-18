namespace InvoiceApp.AI.Common;

/// <summary>
/// Strongly-typed configuration options for Azure OpenAI services.
/// Mapped from the "AzureOpenAI" configuration section in appsettings.json.
/// </summary>
/// <remarks>
/// Contains connection parameters required to initialize <see cref="Azure.AI.OpenAI.AzureOpenAIClient"/>
/// and Microsoft.Extensions.AI chat clients for live LLM completions and embeddings.
/// </remarks>
public sealed class AzureOpenAISettings
{
    /// <summary>
    /// The configuration section name within application configuration files (e.g., appsettings.json).
    /// </summary>
    public const string SectionName = "AzureOpenAI";

    /// <summary>
    /// Gets or sets the HTTPS endpoint URL for the Azure OpenAI resource.
    /// Example: "https://your-resource-name.openai.azure.com/".
    /// </summary>
    public string Endpoint { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the Azure OpenAI API secret key used for authentication.
    /// When empty or set to placeholder values, the system activates Simulation Mode.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the name of the chat completion model deployment in Azure OpenAI.
    /// Defaults to "gpt-4o".
    /// </summary>
    public string DeploymentName { get; set; } = "gpt-4o";

    /// <summary>
    /// Gets or sets the name of the text embedding model deployment in Azure OpenAI.
    /// Defaults to "text-embedding-3-small".
    /// </summary>
    public string EmbeddingDeploymentName { get; set; } = "text-embedding-3-small";
}
