// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Represents the management functionality for group chat interactions
///     within the agent workflow system. This abstract class serves as a
///     base for implementing custom logic for managing group chat sessions,
///     leveraging the functionalities provided by the parent
///     AgentWorkflowBuilder.GroupChatManager class.
/// </summary>
public abstract class AgentGroupChatManager : AgentWorkflowBuilder.RoundRobinGroupChatManager
{
    /// <summary>
    ///     Represents the management functionality for group chat interactions
    ///     within the agent workflow system. This abstract class extends
    ///     the functionality of the RoundRobinGroupChatManager provided by the
    ///     AgentWorkflowBuilder and facilitates managing group chat sessions
    ///     in a custom implementation.
    /// </summary>
    public AgentGroupChatManager(IReadOnlyList<AIAgent> agents)
        : base(agents)
    {
    }
}