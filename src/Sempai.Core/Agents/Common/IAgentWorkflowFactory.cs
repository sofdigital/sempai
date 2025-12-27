// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Extensions.AI;

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Defines a factory for creating workflows that manage interactions between agents.
/// </summary>
/// <remarks>
///     This interface provides methods to create different types of agent workflows, including concurrent,
///     handoff, and sequential workflows. Each method allows customization of the workflow behavior  through parameters
///     such as agents, aggregators, or handoff configurations.
/// </remarks>
public interface IAgentWorkflowFactory
{
    /// <summary>
    ///     Creates a concurrent workflow for processing messages using the specified agents.
    /// </summary>
    /// <param name="agents">A collection of agents that will process messages concurrently. Cannot be null or empty.</param>
    /// <param name="aggregator">
    ///     An optional function to aggregate the results from all agents. If null, a default aggregation strategy is used.
    ///     The function takes a list of message lists (one per agent) and returns a single aggregated list of messages.
    /// </param>
    /// <returns>An instance of <see cref="IAgentWorkflow" /> that represents the concurrent workflow.</returns>
    IAgentWorkflow CreateConcurrent(
        IEnumerable<IAgent> agents,
        Func<IList<List<ChatMessage>>, List<ChatMessage>>? aggregator = null);

    /// <summary>
    ///     Creates a group chat workflow for processing messages with the specified agents.
    /// </summary>
    /// <param name="maxIterations">
    ///     The maximum number of iterations allowed for the group chat workflow. Default is 3.
    ///     Must be a positive integer.
    /// </param>
    /// <param name="agents">
    ///     A variable-length array of agents participating in the group chat.
    ///     Must contain at least one agent. Cannot be null or empty.
    /// </param>
    /// <returns>An instance of <see cref="IAgentWorkflow" /> representing the group chat workflow.</returns>
    public IAgentWorkflow CreateGroupChat(
        int maxIterations = 3,
        params IAgent[] agents);

    /// <summary>
    ///     Creates a group chat workflow for agents with the specified configuration.
    /// </summary>
    /// <param name="maxIterations">
    ///     The maximum number of iterations allowed for the group chat workflow. Default is 3.
    /// </param>
    /// <param name="customManagerFactory"></param>
    /// <param name="agents">
    ///     A collection of agents that participate in the group chat. At least one agent must be provided.
    /// </param>
    /// <returns>An instance of <see cref="IAgentWorkflow" /> representing the group chat workflow.</returns>
    public IAgentWorkflow CreateGroupChat<T>(
        int maxIterations = 3,
        Func<IReadOnlyList<IAgent>, T>? customManagerFactory = null,
        params IAgent[] agents) where T : AgentGroupChatManager;

    /// <summary>
    ///     Creates a handoff workflow for transitioning between agents.
    /// </summary>
    /// <remarks>
    ///     This method allows for the creation of a workflow where agents can transition
    ///     responsibilities based on the provided handoff specifications. The workflow starts with the
    ///     <paramref
    ///         name="initialAgent" />
    ///     and progresses through the specified handoff configurations.
    /// </remarks>
    /// <param name="initialAgent">The initial agent that will handle the workflow before any handoff occurs.</param>
    /// <param name="handoffSpecifications">
    ///     An array of configurations specifying the conditions and parameters for agent
    ///     handoffs.
    /// </param>
    /// <returns>An <see cref="IAgentWorkflow" /> instance representing the configured handoff workflow.</returns>
    IAgentWorkflow CreateHandoff(
        IAgent initialAgent,
        params AgentHandoffConfiguration[] handoffSpecifications);

    /// <summary>
    ///     Creates a sequential workflow using the specified agents.
    /// </summary>
    /// <remarks>
    ///     This method constructs a workflow where the specified agents are executed sequentially.
    ///     Ensure that the agents provided are in the desired execution order.
    /// </remarks>
    /// <param name="agents">
    ///     The agents to include in the sequential workflow. Each agent
    ///     will be executed in the order provided.
    /// </param>
    /// <returns>An <see cref="IAgentWorkflow" /> representing the sequential workflow.</returns>
    IAgentWorkflow CreateSequential(params IAgent[] agents);
}