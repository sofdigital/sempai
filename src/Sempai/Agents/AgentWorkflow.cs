// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using SofDigital.Sempai.Core;
using SofDigital.Sempai.Core.Agents;

namespace SofDigital.Sempai.Agents;

/// <inheritdoc />
public class AgentWorkflow(ILogger<AgentWorkflow>? logger = null) : IAgentWorkflow
{
    /// <summary>
    ///     Represents the ongoing streaming execution of a workflow instance. This variable is used
    ///     to manage and monitor the progress of a workflow execution that produces streamed outputs,
    ///     allowing for real-time interaction and processing of events or messages.
    /// </summary>
    private StreamingRun? _streamingRun;

    /// <summary>
    ///     Gets or sets the workflow object that defines the series of operations or processes
    ///     to be carried out. This property represents the core definition and structure
    ///     of the workflow, which can be executed or streamed to handle routing, task allocation,
    ///     and interaction among agents.
    /// </summary>
    public Workflow? Workflow { get; set; }

    /// <summary>
    ///     Gets or sets the collection of agents involved in the workflow.
    ///     This property represents a sequence of agents participating in the operations
    ///     defined by the workflow, allowing for interaction and execution of tasks
    ///     based on the workflow type and configuration.
    /// </summary>
    public IEnumerable<IAgent> Agents { get; set; }

    /// <summary>
    ///     Gets or sets the handoff sequence among agents in the workflow.
    ///     This property represents a collection of agent groups, where each group defines
    ///     a set of agents involved in a specific handoff stage of the workflow.
    /// </summary>
    public IEnumerable<IEnumerable<IAgent>> Handoffs { get; set; }

    /// <summary>
    ///     Gets or sets the type of workflow for an agent.
    ///     This property determines the mode of operation for the workflow,
    ///     which can be one of the predefined values in the <c>AgentWorkflowType</c> enumeration.
    /// </summary>
    public AgentWorkflowType WorkflowType { get; set; }


    /// <inheritdoc />
    public async Task ConsumeStreamAsync(Action<ChatMessage> onMessage, CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(onMessage);

        await foreach (var message in GetStreamAsync().WithCancellation(cancellationToken).ConfigureAwait(false))
            onMessage(message);
    }

    /// <inheritdoc />
    public async Task<bool> CreateStreamAsync(IEnumerable<ChatMessage> messages, bool? events = null)
    {
        if (Workflow == null || _streamingRun != null) return false;

        _streamingRun = await InProcessExecution.StreamAsync(Workflow!, messages);

        await _streamingRun.TrySendMessageAsync(new TurnToken(events));

        return true;
    }

    /// <inheritdoc />
    public async IAsyncEnumerable<ChatMessage> GetStreamAsync(bool includeUpdates = false)
    {
        if (Workflow == null || _streamingRun == null) yield break;

        await foreach (var evt in _streamingRun.WatchStreamAsync().ConfigureAwait(false))
            switch (evt)
            {
                case AgentRunUpdateEvent updateEvent:
                    logger?.LogInformation("{EExecutorId}: {EData}", updateEvent.ExecutorId, updateEvent.Data);
                    if (includeUpdates)
                        foreach (var message in updateEvent.AsResponse().Messages)
                            yield return message;
                    break;
                case WorkflowOutputEvent outputEvent:
                {
                    var messages = (List<ChatMessage>)outputEvent.Data!;
                    foreach (var message in messages) yield return message;
                    yield break;
                }
                case WorkflowErrorEvent errorEvent:
                    logger?.LogError($"Workflow Error Data: {errorEvent.Data}");
                    break;
                default:
                    logger?.LogError($"Unhandled Event: {evt.GetType().Name}");
                    break;
            }
    }
}