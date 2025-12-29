// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Extensions.AI;

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Defines a factory for creating agent instances based on a specified configuration.
/// </summary>
/// <remarks>
///     Implementations of this interface are responsible for instantiating agents of various types using the
///     provided configuration. The factory may support asynchronous creation and can be extended to accommodate different
///     agent lifecycles or initialization requirements.
/// </remarks>
public interface IAgentFactory
{
    /// <summary>
    ///     Creates and initializes a new agent instance of the specified type using the provided configuration.
    /// </summary>
    /// <typeparam name="T">The type of agent to create. Must inherit from AgentBase and implement IAgent.</typeparam>
    /// <param name="connector"></param>
    /// <param name="configuration">The configuration settings to use for initializing the agent. Cannot be null.</param>
    /// <returns>
    ///     A task that represents the asynchronous creation of the agent. The result contains the initialized agent
    ///     instance, or null if creation fails.
    /// </returns>
    public Task<T?> CreateAgent<T>(AgentConnector? connector = null, AgentConfiguration? configuration = null)
        where T : AgentBase, IAgent;

    /// <summary>
    ///     Creates and initializes a new function instance with the specified parameters.
    /// </summary>
    /// <param name="method"></param>
    /// <param name="name"></param>
    /// <param name="description"></param>
    /// <returns>
    ///     A task that represents the asynchronous creation of the function. The result contains the initialized function
    ///     instance, or null if creation fails.
    /// </returns>
    public AIFunction CreateFunction(Delegate method, string name, string description);
}