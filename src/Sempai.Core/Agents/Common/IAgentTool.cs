// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Extensions.AI;

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Defines the contract for an agent tool, providing access to its name and the underlying tool instance.
/// </summary>
public interface IAgentTool
{
    /// <summary>
    ///     Gets or sets the description of the tool's functionality.
    /// </summary>
    /// <remarks>
    ///     This property provides a brief explanation of what the tool does. It is typically used for documentation
    ///     or to display information about the tool in user interfaces or logs.
    /// </remarks>
    public string Description { get; set; }

    /// <summary>
    ///     Gets or sets the parameters accepted by the tool during execution.
    /// </summary>
    /// <remarks>
    ///     This property specifies the list of parameters that the tool can process.
    ///     It is intended to provide flexibility for customizing tool behavior or inputs.
    ///     The value is optional and can be null if the tool does not require any parameters.
    /// </remarks>
    public IEnumerable<string>? Parameters { get; set; }

    /// <summary>
    ///     Gets or sets the name of the tool associated with the agent.
    /// </summary>
    public string ToolName { get; set; }

    /// <summary>
    ///     Gets the underlying AI tool instance.
    /// </summary>
    /// <returns>The AI tool instance.</returns>
    AITool GetTool();
}