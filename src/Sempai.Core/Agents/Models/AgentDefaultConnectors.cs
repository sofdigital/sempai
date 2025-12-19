// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Represents the default connectors available for an agent, categorized by service tiers or functionalities.
/// </summary>
/// <remarks>
///     This class provides properties to access connectors for different levels of service, such as Basic,
///     Standard, Premium,  and Reasoning. Each property corresponds to a specific type of connector that may be used to
///     configure or interact  with the agent's capabilities.
/// </remarks>
public class AgentDefaultConnectors
{
    /// <summary>
    ///     Gets or sets the connector that provides basic service functionality for the agent.
    /// </summary>
    /// <remarks>
    ///     This property is used to configure or retrieve the default "Basic" tier connector
    ///     associated with the agent. The "Basic" connector offers foundational capabilities
    ///     and is typically used for lightweight or less resource-intensive operations.
    /// </remarks>
    public AgentConnector? Basic { get; set; }

    /// <summary>
    ///     Gets or sets the connector that provides standard service functionality for the agent.
    /// </summary>
    /// <remarks>
    ///     This property is used to configure or retrieve the "Standard" tier connector
    ///     associated with the agent. The "Standard" connector provides enhanced capabilities
    ///     compared to the "Basic" tier, offering a balanced approach between performance
    ///     and resource usage.
    /// </remarks>
    public AgentConnector? Standard { get; set; }

    /// <summary>
    ///     Gets or sets the connector that provides premium service functionality for the agent.
    /// </summary>
    /// <remarks>
    ///     This property is used to configure or retrieve the "Premium" tier connector
    ///     associated with the agent. The "Premium" connector offers enhanced capabilities,
    ///     typically suited for advanced or resource-intensive operations that require
    ///     more sophisticated processing and higher performance.
    /// </remarks>
    public AgentConnector? Premium { get; set; }


    /// <summary>
    ///     Gets or sets the connector that provides advanced reasoning and cognitive service functionality for the agent.
    /// </summary>
    /// <remarks>
    ///     This property is used to configure or retrieve the "Reasoning" tier connector associated with the agent.
    ///     The "Reasoning" connector is designed for complex, logic-driven operations, enabling the agent to perform
    ///     sophisticated decision-making and problem-solving tasks.
    /// </remarks>
    /// ßß
    public AgentConnector? Reasoning { get; set; }
}