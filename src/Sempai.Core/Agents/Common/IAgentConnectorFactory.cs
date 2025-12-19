// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Factory class for creating <see cref="AgentConnector" /> instances for various AI providers.
/// </summary>
public interface IAgentConnectorFactory
{
    /// <summary>
    ///     Builds an <see cref="AgentConnector" /> for Anthropic.
    /// </summary>
    /// <param name="apiKey">The API key for Anthropic.</param>
    /// <param name="model">The model name.</param>
    /// <returns>An <see cref="AgentConnector" /> configured for Anthropic.</returns>
    public AgentConnector BuildAnthropicConnector(string apiKey, string model);

    /// <summary>
    ///     Builds an <see cref="AgentConnector" /> for Azure AI Foundry.
    /// </summary>
    /// <param name="apiKey">The API key for Azure AI Foundry.</param>
    /// <param name="model">The model name.</param>
    /// <param name="resourceUri">The resource URI for Azure AI Foundry.</param>
    /// <returns>An <see cref="AgentConnector" /> configured for Azure AI Foundry.</returns>
    public AgentConnector BuildAzureAIFoundryConnector(string apiKey, string model, string resourceUri);

    /// <summary>
    ///     Builds an <see cref="AgentConnector" /> for Azure OpenAI.
    /// </summary>
    /// <param name="apiKey">The API key for Azure OpenAI.</param>
    /// <param name="model">The model name.</param>
    /// <param name="resourceUri">The resource URI for Azure OpenAI.</param>
    /// <returns>An <see cref="AgentConnector" /> configured for Azure OpenAI.</returns>
    public AgentConnector BuildAzureOpenAIConnector(string apiKey, string model, string resourceUri);

    /// <summary>
    ///     Builds an <see cref="AgentConnector" /> for Google Gemini.
    /// </summary>
    /// <param name="apiKey">The API key for Google Gemini.</param>
    /// <param name="model">The model name.</param>
    /// <returns>An <see cref="AgentConnector" /> configured for Google Gemini.</returns>
    public AgentConnector BuildGoogleGeminiConnector(string apiKey, string model);

    /// <summary>
    ///     Builds an <see cref="AgentConnector" /> for Ollama.
    /// </summary>
    /// <param name="model">The model name.</param>
    /// <param name="resourceUri">The resource URI for Ollama.</param>
    /// <returns>An <see cref="AgentConnector" /> configured for Ollama.</returns>
    public AgentConnector BuildOllamaConnector(string model, string resourceUri);

    /// <summary>
    ///     Builds an <see cref="AgentConnector" /> for OpenAI.
    /// </summary>
    /// <param name="apiKey">The API key for OpenAI.</param>
    /// <param name="model">The model name.</param>
    /// <returns>An <see cref="AgentConnector" /> configured for OpenAI.</returns>
    public AgentConnector BuildOpenAIConnector(string apiKey, string model);

    /// <summary>
    ///     Builds an <see cref="AgentConnector" /> for XAI.
    /// </summary>
    /// <param name="apiKey">The API key for XAI.</param>
    /// <param name="model">The model name.</param>
    /// <returns>An <see cref="AgentConnector" /> configured for XAI.</returns>
    public AgentConnector BuildXAIConnector(string apiKey, string model);
}