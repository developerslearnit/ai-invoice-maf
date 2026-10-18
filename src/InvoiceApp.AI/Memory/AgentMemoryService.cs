using System.Collections.Concurrent;

namespace InvoiceApp.AI.Memory;

/// <summary>
/// Defines persistence and retrieval operations for agent conversation memory across multi-turn sessions.
/// </summary>
public interface IAgentMemoryService
{
    /// <summary>
    /// Retrieves recent chronological conversation history for the specified conversation session.
    /// </summary>
    /// <param name="conversationId">The unique session identifier for the conversation.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A formatted string representing previous conversation turns, or empty if none exists.</returns>
    Task<string> GetConversationHistoryAsync(string conversationId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Appends a single message turn (user or agent) to the conversation history store.
    /// </summary>
    /// <param name="conversationId">The unique session identifier for the conversation.</param>
    /// <param name="role">The speaker role (e.g. "User", "Assistant", "Coordinator").</param>
    /// <param name="content">The textual message content of the turn.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous append operation.</returns>
    Task AppendMessageAsync(string conversationId, string role, string content, CancellationToken cancellationToken = default);
}

/// <summary>
/// In-memory thread-safe implementation of <see cref="IAgentMemoryService"/> using a <see cref="ConcurrentDictionary{TKey, TValue}"/>.
/// </summary>
/// <remarks>
/// Keeps an in-memory sliding window of the last 10 turns per conversation ID.
/// Suitable for development, demonstration, and single-instance deployments. Can be substituted
/// with Redis, Cosmos DB, or SQL Server persistence for distributed multi-node clusters.
/// </remarks>
public sealed class AgentMemoryService : IAgentMemoryService
{
    private static readonly ConcurrentDictionary<string, List<string>> _store = new();

    /// <inheritdoc />
    public Task<string> GetConversationHistoryAsync(string conversationId, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(conversationId))
        {
            return Task.FromResult(string.Empty);
        }

        if (_store.TryGetValue(conversationId, out var messages))
        {
            lock (messages)
            {
                return Task.FromResult(string.Join("\n", messages.TakeLast(10)));
            }
        }

        return Task.FromResult(string.Empty);
    }

    /// <inheritdoc />
    public Task AppendMessageAsync(string conversationId, string role, string content, CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(conversationId))
        {
            return Task.CompletedTask;
        }

        var list = _store.GetOrAdd(conversationId, _ => []);
        lock (list)
        {
            list.Add($"[{role}]: {content}");
        }

        return Task.CompletedTask;
    }
}
