// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Defines a contract for applying agent-specific parameters to a consumer instance.
/// </summary>
/// <remarks>
///     Implementations of this interface use the provided parameters to configure or modify their behavior.
///     This interface is typically used to decouple parameter configuration from agent logic, allowing flexible parameter
///     injection.
/// </remarks>
/// <typeparam name="TParameters">The type of agent parameters to apply. Must implement <see cref="IAgentParameters" />.</typeparam>
public interface IAgentParameterConsumer<in TParameters>
    where TParameters : class, IAgentParameters
{
    /// <summary>
    ///     Apply the specified parameters to the consumer instance.
    /// </summary>
    /// <param name="parameters"></param>
    void ApplyParameters(TParameters parameters);
}