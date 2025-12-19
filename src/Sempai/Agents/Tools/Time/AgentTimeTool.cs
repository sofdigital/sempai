// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using SofDigital.Sempai.Core.Agents;

namespace SofDigital.Sempai.Agents.Tools.Time;

/// <summary>
///     Provides functionality to retrieve the current system time in a formatted string.
/// </summary>
/// <remarks>
///     This tool is designed to return the current system time in ISO 8601 format. It is primarily used in
///     scenarios where the current time needs to be retrieved and logged or displayed. The tool logs its execution details
///     for debugging purposes.
/// </remarks>
/// <param name="logger"></param>
public sealed class AgentTimeTool(ILogger<AgentTimeTool> logger) : IAgentTool
{
    private readonly ILogger<AgentTimeTool> _logger = logger ?? throw new ArgumentNullException(nameof(logger));

    /// <inheritdoc />
    public string Description { get; set; } = "Returns the current time";

    /// <inheritdoc />
    public string ToolName { get; set; } = "get_current_time";

    /// <inheritdoc />
    public IEnumerable<string>? Parameters { get; set; } = null;

    /// <summary>
    ///     Retrieves the underlying AI tool instance associated with the current implementation.
    /// </summary>
    /// <returns>The AI tool instance.</returns>
    /// <exception cref="NotImplementedException">Thrown when the method is not implemented.</exception>
    public AITool GetTool()
    {
        throw new NotImplementedException();
    }

    /// <summary>
    ///     Executes the tool logic and returns the current system time as a string in ISO 8601 format.
    /// </summary>
    /// <param name="input">A string input, not utilized in the current implementation.</param>
    /// <returns>A task representing the asynchronous operation, containing the current system time as a formatted string.</returns>
    public Task<string> ExecuteAsync(string input)
    {
        _logger.LogDebug("AgentTimeTool.ExecuteAsync invoked with input: {Input}", input);

        var result = $"The current system time is {DateTime.Now:O}";

        _logger.LogDebug("AgentTimeTool returning: {Result}", result);

        return Task.FromResult(result);
    }
}