// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using SofDigital.Sempai.Core.Agents;

namespace SofDigital.Sempai.Agents;

/// <summary>
///     Represents the configuration parameters for an agent, including indexing, filtering, and result limits.
/// </summary>
/// <remarks>
///     This class provides configurable options for an agent, such as the name of the index to query,  the
///     maximum number of results to return, and an optional filter to apply to the query.
/// </remarks>
public class ConfigurableAgentParameters
    : IAgentParameters
{
    /// <summary>
    ///     Gets or sets the name of the index to query.
    /// </summary>
    /// <remarks>
    ///     This property specifies the name of the index used by the agent during its operation.
    ///     It plays a crucial role in determining the source of the data for query execution.
    /// </remarks>
    public string? IndexName { get; set; }

    /// <summary>
    ///     Gets the maximum number of results to return from a query.
    /// </summary>
    /// <remarks>
    ///     This property defines the upper limit of results that the agent can retrieve during execution.
    ///     It is useful for controlling the amount of data returned and optimizing performance.
    ///     The default value is set to 5.
    /// </remarks>
    public int TopK { get; init; } = 5;

    /// <summary>
    ///     Gets or sets an optional filter expression to apply to the query.
    /// </summary>
    public string? Filter { get; init; }
}