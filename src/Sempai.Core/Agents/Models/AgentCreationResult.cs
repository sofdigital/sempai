// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Agents.AI;

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Represents the result of creating a chat client agent, including the agent instance and optional run options.
/// </summary>
/// <param name="Agent">The created <see cref="ChatClientAgent" /> instance.</param>
/// <param name="RunOptions">Optional run options for the agent, or <c>null</c> if not specified.</param>
public sealed record AgentCreationResult(ChatClientAgent Agent, ChatClientAgentRunOptions? RunOptions);