// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Represents the configuration required to connect to an AI agent provider,
///     including authentication credentials and model selection.
/// </summary>
public class AgentConnector
{
    /// <summary>
    ///     Encapsulates the necessary details to establish a connection with an AI agent provider,
    ///     including provider type, authentication key, model specifications, and optional resource URI.
    /// </summary>
    public AgentConnector()
    {
    }

    /// <summary>
    ///     Represents a connector for interacting with an agent using the specified provider, API key, model, and optional
    ///     resource URI.
    /// </summary>
    public AgentConnector(
        AgentProviderType provider,
        string apiKey,
        string model,
        string? resourceUri)
    {
        Provider = provider;
        ApiKey = apiKey;
        Model = model;
        ResourceUri = resourceUri;
    }

    /// <summary>
    ///     Gets or sets the API key used for authenticating requests to the AI agent provider.
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the identifier of the AI model to be used for processing requests.
    /// </summary>
    public string Model { get; set; } = string.Empty;

    /// <summary>
    ///     Gets or sets the provider type for the AI connection, indicating the specific AI service or platform to be used.
    /// </summary>
    public AgentProviderType Provider { get; set; }

    /// <summary>
    ///     Gets or sets the resource URI used to specify the endpoint or base address
    ///     for connecting to the AI agent provider.
    /// </summary>
    public string? ResourceUri { get; set; }
}