// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace SofDigital.Sempai.Core;

/// <summary>
///     Specifies the workflow type for an agent in a system or process.
/// </summary>
/// <remarks>
///     This enumeration defines the possible workflows that an agent can follow:
///     <list type="bullet">
///         <item>
///             <term>Concurrent</term>
///             <description>
///                 Indicates that the agent can handle multiple tasks
///                 simultaneously.
///             </description>
///         </item>
///         <item>
///             <term>Handoff</term>
///             <description>
///                 Indicates that the agent transfers
///                 tasks to another agent or system for further processing.
///             </description>
///         </item>
///         <item>
///             <term>Sequential</term>
///             <description>Indicates that the agent processes tasks one at a time in a defined order.</description>
///         </item>
///     </list>
/// </remarks>
public enum AgentWorkflowType
{
    /// <summary>
    ///     Represents a workflow type where an agent can handle multiple tasks or processes simultaneously.
    /// </summary>
    /// <remarks>
    ///     This workflow type allows agents to operate in parallel, managing concurrent tasks effectively.
    ///     It is used in scenarios where multitasking or parallel processing is required to enhance efficiency.
    /// </remarks>
    Concurrent,

    /// <summary>
    ///     Represents a workflow type where tasks or responsibilities are transferred from one agent or system to another.
    /// </summary>
    /// <remarks>
    ///     This workflow type is typically used in scenarios where collaboration or delegation is necessary.
    ///     The agent transfers tasks for further processing by a different entity, which could be another agent or an
    ///     automated system.
    ///     It is designed to facilitate seamless task transitions while maintaining efficiency and responsibility handover
    ///     integrity.
    /// </remarks>
    Handoff,

    /// <summary>
    ///     Represents a workflow type where an agent processes tasks sequentially, one at a time, in a defined order.
    /// </summary>
    /// <remarks>
    ///     This workflow type ensures tasks are managed in a linear, step-by-step manner.
    ///     It is used in scenarios where maintaining task order or focusing on single-task processing
    ///     is essential for accuracy and consistency.
    /// </remarks>
    Sequential,

    /// <summary>
    /// Represents a workflow type where an agent participates in managing group chat interactions.
    /// </summary>
    /// <remarks>
    /// This workflow type is designed for handling group conversations, enabling agents to interact with multiple users within a shared communication session.
    /// It is commonly used in scenarios involving collaboration or community-based engagements.
    /// </remarks>
    GroupChat,
    /// <summary>
    /// Represents a workflow type where tasks or interactions are naturally drawn to an agent for resolution.
    /// </summary>
    /// <remarks>
    /// This workflow type is designed to prioritize the allocation of tasks or interactions to an agent based on a matching or attraction mechanism.
    /// It is typically used in scenarios where alignment of specific skills, context, or preferences play a critical role in task assignment.
    /// </remarks>
    Magnetic
}