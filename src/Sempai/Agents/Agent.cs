// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using System.ComponentModel;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using SofDigital.Sempai.Core;
using SofDigital.Sempai.Core.Agents;

namespace SofDigital.Sempai.Agents;

/// <summary>
///     Represents a generic agent that provides AI tools and functionality, including retrieving the current date and
///     time.
/// </summary>
/// <remarks>
///     This class extends <see cref="AgentBase" /> and implements the <see cref="IAgent" /> interface. It
///     provides specific tools that can be used in AI-related workflows, such as retrieving the current date and time in
///     either local or UTC format.
/// </remarks>
public partial class Agent(ILogger<Agent> logger)
    : AgentBase, IAgent
{
    private readonly ILogger<Agent> _logger = logger;

    /// <inheritdoc />
    public override AIFunction? GetAsAgentTool()
    {
        return null;
    }

    /// <summary>
    ///     Retrieves a collection of AI tools available for use by the agent.
    /// </summary>
    /// <returns>
    ///     An enumerable collection of <see cref="AITool" /> instances that represent
    ///     the tools provided by the agent. Each tool encapsulates a specific functionality,
    ///     such as retrieving the current date and time.
    /// </returns>
    public override IEnumerable<AITool> GetTools()
    {
        return [AIFunctionFactory.Create(GetCurrentDateAndTimeTool, "current_date_and_time")];
    }

    /// <summary>
    ///     Retrieves the current date and time based on the specified time type.
    /// </summary>
    /// <param name="timeType">
    ///     A <see cref="TimeType" /> value that specifies whether to retrieve the date and time
    ///     in either local or UTC format.
    /// </param>
    /// <returns>
    ///     A <see cref="DateTime" /> object representing the current date and time in the specified format.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    ///     Thrown when the provided <paramref name="timeType" /> is not a valid <see cref="TimeType" /> value.
    /// </exception>
    [Description("Get current the date and time")]
    public DateTime GetCurrentDateAndTimeTool([Description("Local or UTC time")] TimeType timeType)
    {
        LogGetCurrentDateAndTimeToolCalledWithTimeTypeByAgentName(timeType, ChatClientAgent!.Name!);

        return timeType switch
        {
            TimeType.Local => DateTime.Now,
            TimeType.Utc => DateTime.UtcNow,
            _ => throw new ArgumentOutOfRangeException(nameof(timeType), timeType, null)
        };
    }

    /// <summary>
    ///     Logs an informational message when the GetCurrentDateAndTimeTool method is called, including the specified time
    ///     type and the agent name.
    /// </summary>
    /// <param name="timeType">
    ///     A <see cref="TimeType" /> value indicating whether the date and time requested are in local or UTC format.
    /// </param>
    /// <param name="agentName">
    ///     A string representing the name of the agent that invoked the GetCurrentDateAndTimeTool method.
    /// </param>
    [LoggerMessage(LogLevel.Information, "GetCurrentDateAndTimeTool called with TimeType: {timeType} by {agentName}")]
    partial void LogGetCurrentDateAndTimeToolCalledWithTimeTypeByAgentName(TimeType timeType, string agentName);
}