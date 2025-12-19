// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Extensions.AI;
using SofDigital.Sempai.Core.Agents;

namespace SofDigital.Sempai.Agents;

/// <summary>
///     Represents an agent that can be configured with specific parameters and provides tools or functionality based on
///     those parameters.
/// </summary>
/// <remarks>
///     This class extends <see cref="AgentBase" /> and implements <see cref="IAgent" /> and
///     <see
///         cref="IAgentParameterConsumer{T}" />
///     to allow parameterized configuration.  Use
///     <see
///         cref="ApplyParameters(ConfigurableAgentParameters)" />
///     to configure the agent before invoking its
///     functionality.
/// </remarks>
public class ConfigurableAgent
    : AgentBase, IAgent, IAgentParameterConsumer<ConfigurableAgentParameters>
{
    private ConfigurableAgentParameters? _parameters;

    /// <inheritdoc />
    public override AIFunction? GetAsAgentTool()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public override IEnumerable<AITool> GetTools()
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public void ApplyParameters(ConfigurableAgentParameters parameters)
    {
        _parameters = parameters;
    }
}