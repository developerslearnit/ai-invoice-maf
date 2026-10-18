using System.ComponentModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;

namespace InvoiceApp.AI.Tools;

/// <summary>
/// Domain tool provider enabling agents to dynamically discover, read, and cite official platform
/// Markdown documentation (RAG ground-truth retrieval).
/// </summary>
/// <remarks>
/// <para>
/// Locates Markdown documentation files stored in the repository's <c>Docs/</c> folder
/// and feeds their full text to agents so they can provide accurate, policy-compliant instructions.
/// </para>
/// </remarks>
/// <param name="logger">Structured logger instance.</param>
public sealed class DocumentationTools(ILogger<DocumentationTools> logger)
{
    private readonly string _docsDirectory = FindDocsDirectory();

    /// <summary>
    /// Generates the list of <see cref="AITool"/> descriptors registered with the LLM via <see cref="AIFunctionFactory.Create"/>.
    /// </summary>
    /// <returns>An enumerable of AI function definitions for documentation retrieval.</returns>
    public IEnumerable<AITool> GetTools()
    {
        yield return AIFunctionFactory.Create(
            ReadDocumentationPage,
            nameof(ReadDocumentationPage),
            "Retrieves markdown documentation on platform features (e.g., 'getting-started', 'creating-customer', 'creating-invoice', 'managing-invoices', 'viewing-invoices', 'dashboard-and-metrics', 'ai-assistant').");
    }

    /// <summary>
    /// Reads and returns the complete text of an official platform Markdown documentation file.
    /// </summary>
    /// <param name="pageName">The slug or filename of the documentation page without file extension (e.g. 'creating-customer', 'creating-invoice').</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>The raw Markdown content of the documentation page, or an available index if not found.</returns>
    [Description("Retrieves the content of a platform documentation page.")]
    public async Task<string> ReadDocumentationPage(
        [Description("Page name without extension (e.g. 'creating-customer', 'creating-invoice', 'managing-invoices', 'getting-started')")] string pageName,
        CancellationToken cancellationToken = default)
    {
        logger.LogInformation("Tool invoked: ReadDocumentationPage for '{PageName}'", pageName);

        if (string.IsNullOrWhiteSpace(pageName))
        {
            return "Documentation page name cannot be empty.";
        }

        var safeName = Path.GetFileNameWithoutExtension(pageName.Trim());
        var filePath = Path.Combine(_docsDirectory, $"{safeName}.md");

        if (!File.Exists(filePath))
        {
            logger.LogWarning("Documentation file '{FilePath}' does not exist.", filePath);
            return $"Documentation page '{pageName}' was not found. Available pages: getting-started, creating-customer, managing-customers, creating-invoice, managing-invoices, viewing-invoices, dashboard-and-metrics, ai-assistant.";
        }

        return await File.ReadAllTextAsync(filePath, cancellationToken);
    }

    /// <summary>
    /// Dynamically resolves the absolute path to the Docs directory by searching upward from the application base directory.
    /// </summary>
    /// <returns>The absolute filesystem path to the Docs folder.</returns>
    private static string FindDocsDirectory()
    {
        // 1. Check AppContext.BaseDirectory/Docs
        var localDocs = Path.Combine(AppContext.BaseDirectory, "Docs");
        if (Directory.Exists(localDocs))
        {
            return localDocs;
        }

        // 2. Search upward for Docs directory in solution root
        var dir = new DirectoryInfo(AppContext.BaseDirectory);
        while (dir is not null)
        {
            var candidate = Path.Combine(dir.FullName, "Docs");
            if (Directory.Exists(candidate))
            {
                return candidate;
            }
            dir = dir.Parent;
        }

        return localDocs;
    }
}
