// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Extensions.AI;

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Defines the contract for a workflow consisting of multiple workflow nodes.
/// </summary>
public interface IAgentWorkflow
{
    /// <summary>
    ///     Gets or sets the collection of agents that participate in the workflow.
    ///     Each agent represents a discrete component capable of performing specific tasks
    ///     as part of the workflow execution.
    /// </summary>
    IEnumerable<IAgent> Agents { get; set; }

    /// <summary>
    ///     Gets or sets the collection of agent handoffs within the workflow.
    ///     Each handoff represents a sequence where multiple agents collaborate or pass tasks
    ///     among themselves to advance the workflow's execution.
    /// </summary>
    IEnumerable<IEnumerable<IAgent>> Handoffs { get; set; }

    /// <summary>
    ///     Gets or sets the type of the workflow, specifying how agents interact and process tasks.
    ///     The value indicates whether the workflow is Concurrent, Sequential, or utilizes Handoff behavior.
    /// </summary>
    AgentWorkflowType WorkflowType { get; set; }

    /// <summary>
    ///     Consumes a stream of chat messages asynchronously by processing each message using the provided callback function.
    /// </summary>
    /// <param name="onMessage">
    ///     A callback function invoked for each message in the stream. The function processes the
    ///     incoming {@code ChatMessage}.
    /// </param>
    /// <param name="cancellationToken">
    ///     A cancellation token that can be used to cancel the stream consumption operation.
    ///     Defaults to the default cancellation token if not provided.
    /// </param>
    /// <returns>
    ///     A task representing the asynchronous operation of consuming the message stream.
    /// </returns>
    Task ConsumeStreamAsync(Action<ChatMessage> onMessage, CancellationToken cancellationToken = default);

    /// <summary>
    ///     Creates a stream asynchronously based on the provided chat messages and optional events flag.
    /// </summary>
    /// <param name="messages">The collection of chat messages used to create the stream.</param>
    /// <param name="events">
    ///     An optional parameter indicating whether events should be incorporated into the stream.
    ///     Defaults to null.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains a boolean indicating
    ///     a success or failure of the stream creation.
    /// </returns>
    Task<bool> CreateStreamAsync(IEnumerable<ChatMessage> messages, bool? events = null);

    /// <summary>
    ///     Retrieves a stream of chat messages asynchronously.
    /// </summary>
    /// <returns>An asynchronous stream of <see cref="ChatMessage" /> objects.</returns>
    IAsyncEnumerable<ChatMessage> GetStreamAsync(bool includeUpdates = false);
}