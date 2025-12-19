// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace SofDigital.Sempai.Core;

/// <summary>
///     Specifies the available providers for agent-based AI services.
/// </summary>
/// <remarks>
///     This enumeration defines the supported AI service providers that can be used to configure and
///     interact with agent-based systems.  Each value represents a distinct provider, such as OpenAI or Google Gemini, and
///     may correspond to specific APIs or integrations.
/// </remarks>
public enum AgentProviderType
{
    /// <summary>
    ///     Represents the Anthropic AI service provider.
    /// </summary>
    /// <remarks>
    ///     Anthropic is an AI research company that develops advanced models for agent-based systems.
    ///     This enumeration value is used to configure interactions with Anthropic's services,
    ///     which may involve applications of AI safety and machine learning capabilities.
    /// </remarks>
    Anthropic,

    /// <summary>
    ///     Represents the Azure AI Foundry service provider.
    /// </summary>
    /// <remarks>
    ///     Azure AI Foundry is a suite of AI tools and services provided by Microsoft Azure.
    ///     This enumeration value is used for configuring interactions with Azure AI Foundry,
    ///     enabling integration with Azure's ecosystem of advanced AI solutions.
    /// </remarks>
    AzureAIFoundry,

    /// <summary>
    ///     Represents the Azure OpenAI service provider.
    /// </summary>
    /// <remarks>
    ///     Azure OpenAI combines the capabilities of OpenAI's models with the enterprise-grade features and infrastructure
    ///     of Microsoft Azure. This enumeration value is used to configure interactions with Azure OpenAI services,
    ///     enabling secure and scalable AI applications tailored for enterprise environments.
    /// </remarks>
    AzureOpenAI,

    /// <summary>
    ///     Represents the Google Gemini AI service provider.
    /// </summary>
    /// <remarks>
    ///     Google Gemini is an AI service developed by Google that integrates advanced generative AI capabilities.
    ///     This enumeration value is used to configure interactions with Google's Gemini platform,
    ///     enabling the utilization of its machine learning models and APIs for agent-based systems.
    /// </remarks>
    GoogleGemini,

    /// <summary>
    ///     Represents the Ollama AI service provider.
    /// </summary>
    /// <remarks>
    ///     Ollama is a provider of AI-based services designed for building and deploying interactive agents and models.
    ///     This enumeration value is used to configure interactions with Ollama's platform, enabling integration with its
    ///     advanced AI capabilities for agent-focused solutions.
    /// </remarks>
    Ollama,

    /// <summary>
    ///     Represents the OpenAI service provider.
    /// </summary>
    /// <remarks>
    ///     OpenAI is an artificial intelligence research lab known for developing advanced AI models for natural language
    ///     processing and other tasks.
    ///     This enumeration value is used to specify and configure interactions with OpenAI's services,
    ///     supporting a range of features such as conversational agents, text completion, and more.
    /// </remarks>
    OpenAI,

    /// <summary>
    ///     Represents the XAI service provider.
    /// </summary>
    /// <remarks>
    ///     XAI is an AI service provider focused on delivering explainable and transparent artificial intelligence solutions.
    ///     This enumeration value is used to configure and interact with XAI's offerings, which emphasize interpretability
    ///     and accountability in AI applications.
    /// </remarks>
    XAI
}

/// <summary>
///     Provides extension methods for converting between <see cref="string" /> and <see cref="AgentProviderType" />.
/// </summary>
/// <remarks>
///     This class includes methods to parse a string into an <see cref="AgentProviderType" /> enumeration
///     value and to convert an <see cref="AgentProviderType" /> value back to its string representation.
/// </remarks>
public static class AgentProviderTypeExtensions
{
    /// <summary>
    ///     Converts the specified string value to its corresponding <see cref="AgentProviderType" /> enumeration value.
    /// </summary>
    /// <param name="value">The string representation of the <see cref="AgentProviderType" /> to convert.</param>
    /// <returns>The corresponding <see cref="AgentProviderType" /> enumeration value.</returns>
    /// <exception cref="ArgumentNullException">Thrown when the <paramref name="value" /> is null or empty.</exception>
    /// <exception cref="ArgumentException">
    ///     Thrown when the <paramref name="value" /> does not match any valid
    ///     <see cref="AgentProviderType" />.
    /// </exception>
    public static AgentProviderType ToAgentProviderType(this string value)
    {
        if (string.IsNullOrWhiteSpace(value)) throw new ArgumentNullException(nameof(value));

        return Enum.TryParse<AgentProviderType>(value, true, out var result)
            ? result
            : throw new ArgumentException($"Invalid AgentProviderType: {value}", nameof(value));
    }

    /// <summary>
    ///     Converts the specified <see cref="AgentProviderType" /> value to its string representation.
    /// </summary>
    /// <param name="providerType">The <see cref="AgentProviderType" /> value to convert to a string.</param>
    /// <returns>The string representation of the specified <see cref="AgentProviderType" /> value.</returns>
    public static string ToAgentProviderTypeString(this AgentProviderType providerType)
    {
        return providerType.ToString();
    }
}