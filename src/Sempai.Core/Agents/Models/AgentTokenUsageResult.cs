// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Represents the result of token usage in an agent's operation,
///     encapsulating the count of tokens used for input, output, and reasoning processes.
/// </summary>
public sealed record AgentTokenUsageResult(
    long Input,
    long Output,
    long Reasoning,
    long TotalInput,
    long TotalOutput,
    long TotalReasoning);