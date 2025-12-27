// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Represents the base configuration for agent handoff scenarios, defining the structure of agent-to-agent
///     interactions.
/// </summary>
/// <remarks>
///     This abstract record serves as the foundation for specific handoff configurations, such as
///     one-to-one, one-to-many,  and many-to-one agent interactions. Derived records specify the direction
///     and participants of the handoff,  along with an optional reason for the handoff.
/// </remarks>
public abstract record AgentHandoffConfiguration
{
    /// <summary>
    ///     Represents a one-to-many handoff configuration, where a single agent is handing off to multiple agents.
    /// </summary>
    /// <remarks>
    ///     This configuration is a specialized form of agent handoff, defining the interaction where one source agent
    ///     is delegating responsibility to a collection of target agents. It is part of the agent handoff ecosystem
    ///     that supports various types of coordinated agent-to-agent workflows.
    /// </remarks>
    public sealed record OneToMany(IAgent From, IEnumerable<IAgent> To)
        : AgentHandoffConfiguration;

    /// <summary>
    ///     Defines a configuration for many-to-one agent handoff scenarios, where multiple agents hand off their interactions
    ///     to a single agent.
    /// </summary>
    /// <remarks>
    ///     This record is used to represent the many-to-one direction of agent handoff, ensuring that multiple
    ///     originating agents can be mapped to a single receiving agent. An optional handoff reason provides context for
    ///     the transfer.
    /// </remarks>
    public sealed record ManyToOne(IEnumerable<IAgent> From, IAgent To, string? HandoffReason = null)
        : AgentHandoffConfiguration;

    /// <summary>
    ///     Represents a one-to-one agent handoff configuration, specifying a direct interaction
    ///     between a single source agent and a single target agent.
    /// </summary>
    /// <remarks>
    ///     This record is used to define a straightforward handoff scenario where one agent
    ///     directly transfers responsibility or communication to another agent, with
    ///     an optional reason for the handoff.
    /// </remarks>
    public sealed record OneToOne(IAgent From, IAgent To, string? HandoffReason = null)
        : AgentHandoffConfiguration;
}