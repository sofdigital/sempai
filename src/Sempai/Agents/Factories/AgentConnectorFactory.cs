// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using SofDigital.Sempai.Core;
using SofDigital.Sempai.Core.Agents;

namespace SofDigital.Sempai.Agents.Factories;

/// <inheritdoc />
public class AgentConnectorFactory : IAgentConnectorFactory
{
    /// <inheritdoc />
    public AgentConnector BuildAnthropicConnector(string apiKey, string model)
    {
        return new AgentConnector(AgentProviderType.Anthropic, apiKey, model, null);
    }

    /// <inheritdoc />
    public AgentConnector BuildAzureAIFoundryConnector(string apiKey, string model, string resourceUri)
    {
        return new AgentConnector(AgentProviderType.AzureAIFoundry, apiKey, model, resourceUri);
    }

    /// <inheritdoc />
    public AgentConnector BuildAzureOpenAIConnector(string apiKey, string model, string resourceUri)
    {
        return new AgentConnector(AgentProviderType.AzureOpenAI, apiKey, model, resourceUri);
    }

    /// <inheritdoc />
    public AgentConnector BuildGoogleGeminiConnector(string apiKey, string model)
    {
        return new AgentConnector(AgentProviderType.GoogleGemini, apiKey, model, null);
    }

    /// <inheritdoc />
    public AgentConnector BuildOllamaConnector(string model, string resourceUri)
    {
        return new AgentConnector(AgentProviderType.Ollama, "", model, resourceUri);
    }

    /// <inheritdoc />
    public AgentConnector BuildOpenAIConnector(string apiKey, string model)
    {
        return new AgentConnector(AgentProviderType.OpenAI, apiKey, model, null);
    }

    /// <inheritdoc />
    public AgentConnector BuildXAIConnector(string apiKey, string model)
    {
        return new AgentConnector(AgentProviderType.XAI, apiKey, model, null);
    }
}