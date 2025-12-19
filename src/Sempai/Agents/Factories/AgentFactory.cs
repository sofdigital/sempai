// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using System.ClientModel;
using Anthropic.SDK;
using Azure.AI.Agents.Persistent;
using Azure.AI.OpenAI;
using Azure.Identity;
using GenerativeAI.Microsoft;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using OllamaSharp;
using OpenAI;
using SofDigital.Sempai.Core;
using SofDigital.Sempai.Core.Agents;
using SofDigital.Sempai.Core.Configurations;

namespace SofDigital.Sempai.Agents.Factories;

/// <summary>
///     Factory implementation for creating and configuring agent instances based on a specified
///     <see cref="AgentConfiguration" />.
/// </summary>
public class AgentFactory
    : IAgentFactory
{
    /// <summary>
    ///     Logger for agent factory operations and error reporting.
    /// </summary>
    private readonly ILogger<AgentFactory> _logger;

    /// <summary>
    ///     Service provider for resolving agent dependencies.
    /// </summary>
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    ///     Initializes a new instance of <see cref="AgentFactory" />.
    /// </summary>
    /// <param name="logger">Logger instance for diagnostics.</param>
    /// <param name="serviceProvider">Service provider for dependency resolution.</param>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="logger" /> or <paramref name="serviceProvider" /> is
    ///     null.
    /// </exception>
    public AgentFactory(ILogger<AgentFactory> logger, IServiceProvider serviceProvider)
    {
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        _serviceProvider = serviceProvider ?? throw new ArgumentNullException(nameof(serviceProvider));
    }

    /// <inheritdoc />
    public async Task<T?> CreateAgent<T>(AgentConnector? connector = null, AgentConfiguration? configuration = null)
        where T : AgentBase, IAgent
    {
        try
        {
            var agent = _serviceProvider.GetRequiredService<T>();
            var isBuiltIn = agent is IAgentBuiltIn;

            // Resolve effective configuration (fallback to built-in if allowed)
            var effectiveConfiguration = (configuration ?? (isBuiltIn ? agent.AgentConfiguration : null))
                                         ?? throw new ArgumentNullException(nameof(configuration),
                                             "Non built-in agents require a configuration instance.");

            // Apply strongly typed parameters if wrapped
            if (effectiveConfiguration is IConfigurationWrapper { ParametersObject: var parameters })
                ApplyParametersIfSupported(agent, parameters);

            // Resolve effective connector (fallback to built-in if allowed)
            var effectiveConnector = (connector ?? (isBuiltIn ? agent.AgentConnector : null))
                                     ?? throw new ArgumentNullException(nameof(connector),
                                         "Non built-in agents require a connector instance.");

            // Merge agent-specific tools (avoid duplicates)
            var specificTools = agent.GetTools();
            if (specificTools is not null)
            {
                effectiveConfiguration.Tools ??= new List<AITool>();
                foreach (var tool in specificTools)
                    if (!effectiveConfiguration.Tools.Contains(tool))
                        effectiveConfiguration.Tools.Add(tool);
            }

            // Always (re)configure and (re)connect using the resolved effective objects.
            // This ensures user-supplied overrides are applied.
            agent.Configure(effectiveConfiguration);
            agent.Connect(effectiveConnector);

            // Provider-specific agent + run options
            var agentCreationResult = effectiveConnector.Provider switch
            {
                AgentProviderType.Anthropic => BuildAnthropicAgent(effectiveConnector, effectiveConfiguration),
                AgentProviderType.AzureAIFoundry => await BuildAzureAIFoundry(effectiveConnector,
                    effectiveConfiguration),
                AgentProviderType.AzureOpenAI => BuildAzureOpenAIAgent(effectiveConnector, effectiveConfiguration),
                AgentProviderType.GoogleGemini => BuildGoogleGeminiAgent(effectiveConnector, effectiveConfiguration),
                AgentProviderType.Ollama => BuildOllamaAgent(effectiveConnector, effectiveConfiguration),
                AgentProviderType.OpenAI => BuildOpenAIAgent(effectiveConnector, effectiveConfiguration),
                AgentProviderType.XAI => BuildxAIAgent(effectiveConnector, effectiveConfiguration),
                _ => throw new NotSupportedException($"Unsupported provider {effectiveConnector.Provider}")
            };

            agent.Initialize(agentCreationResult.Agent, agentCreationResult.RunOptions);

            return agent;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create ChatClientAgent");
        }

        return null;
    }

    /// <summary>
    ///     Applies strongly typed parameters to the agent if it implements <see cref="IAgentParameterConsumer{TParameters}" />
    /// </summary>
    /// <typeparam name="TAgent">The agent type.</typeparam>
    /// <param name="agent">The agent instance to apply parameters to.</param>
    /// <param name="parameters">The parameters object to apply.</param>
    private static void ApplyParametersIfSupported<TAgent>(TAgent agent, object parameters)
    {
        var paramType = parameters.GetType();
        var consumerInterface = typeof(IAgentParameterConsumer<>).MakeGenericType(paramType);

        if (!consumerInterface.IsInstanceOfType(agent)) return;

        var method = consumerInterface.GetMethod(nameof(IAgentParameterConsumer<IAgentParameters>.ApplyParameters));
        method!.Invoke(agent, [parameters]);
    }

    /// <summary>
    ///     Builds an Anthropic agent and its run options from the specified configuration.
    /// </summary>
    /// <param name="connector">The agent connector</param>
    /// <param name="configuration">The agent configuration.</param>
    /// <returns>The agent creation result containing the agent and run options.</returns>
    private static AgentCreationResult BuildAnthropicAgent(AgentConnector connector, AgentConfiguration configuration)
    {
        var client = new AnthropicClient(
                new APIAuthentication(connector.ApiKey))
            .Messages
            .AsBuilder()
            .Build();

        var agentRunOptions = new ChatClientAgentRunOptions(new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        });

        var agent = new ChatClientAgent(client);

        return new AgentCreationResult(agent, agentRunOptions);
    }

    /// <summary>
    ///     Asynchronously builds an Azure AI Foundry agent and its run options from the specified configuration.
    /// </summary>
    /// <param name="connector">The agent connector.</param>
    /// <param name="configuration">The agent configuration.</param>
    /// <returns>A task containing the agent creation result.</returns>
    private static async Task<AgentCreationResult> BuildAzureAIFoundry(AgentConnector connector,
        AgentConfiguration configuration)
    {
        var client = new PersistentAgentsClient(
            connector.ResourceUri,
#if DEBUG
            new AzureCliCredential());
#else
            new AzureKeyCredential(connector.ApiKey));
#endif

        var aiFoundryAgent = await client.Administration
            .CreateAgentAsync(
                connector.Model,
                configuration.AgentName,
                instructions: configuration.Instructions);

        var agentRunOptions = new ChatClientAgentRunOptions(new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        });

        var agent = await client.GetAIAgentAsync(aiFoundryAgent.Value.Id);

        return new AgentCreationResult(agent, agentRunOptions);
    }

    /// <summary>
    ///     Builds an Azure OpenAI agent and its run options from the specified configuration.
    /// </summary>
    /// <param name="connector">The agent connector.</param>
    /// <param name="configuration">The agent configuration.</param>
    /// <returns>The agent creation result containing the agent and run options.</returns>
    private static AgentCreationResult BuildAzureOpenAIAgent(AgentConnector connector, AgentConfiguration configuration)
    {
        var client = new AzureOpenAIClient(
            new Uri(connector.ResourceUri!),
#if DEBUG
            new DefaultAzureCredential());
#else
            new AzureKeyCredential(connector.ApiKey));
#endif
        var chatClient = client.GetChatClient(connector.Model);

        var agentRunOptions = new ChatClientAgentRunOptions(new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        });

        var agent = chatClient.CreateAIAgent(name: configuration.AgentName);

        return new AgentCreationResult(agent, agentRunOptions);
    }

    /// <summary>
    ///     Builds a Google Gemini agent and its run options from the specified configuration.
    /// </summary>
    /// <param name="connector">The agent connector.</param>
    /// <param name="configuration">The agent configuration.</param>
    /// <returns>The agent creation result containing the agent and run options.</returns>
    private static AgentCreationResult BuildGoogleGeminiAgent(AgentConnector connector,
        AgentConfiguration configuration)
    {
        var client = new GenerativeAIChatClient(
            connector.ApiKey,
            connector.Model);

        var agentRunOptions = new ChatClientAgentRunOptions(new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        });

        var agent = new ChatClientAgent(client, name: configuration.AgentName);

        return new AgentCreationResult(agent, agentRunOptions);
    }

    /// <summary>
    ///     Builds an Ollama agent and its run options from the specified configuration.
    /// </summary>
    /// <param name="connector">The agent connector.</param>
    /// <param name="configuration">The agent configuration.</param>
    /// <returns>The agent creation result containing the agent and run options.</returns>
    private static AgentCreationResult BuildOllamaAgent(AgentConnector connector, AgentConfiguration configuration)
    {
        var client = new OllamaApiClient(
            connector.ResourceUri!,
            connector.Model);

        var agentRunOptions = new ChatClientAgentRunOptions(new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        });

        var agent = new ChatClientAgent(client, name: configuration.AgentName);

        return new AgentCreationResult(agent, agentRunOptions);
    }

    /// <summary>
    ///     Builds an OpenAI agent and its run options from the specified configuration.
    /// </summary>
    /// <param name="connector">The agent connector.</param>
    /// <param name="configuration">The agent configuration.</param>
    /// <returns>The agent creation result containing the agent and run options.</returns>
    private static AgentCreationResult BuildOpenAIAgent(AgentConnector connector, AgentConfiguration configuration)
    {
        var client = new OpenAIClient(connector.ApiKey);

        var agentRunOptions = new ChatClientAgentRunOptions(new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        });

        var agent = client.GetChatClient(connector.Model).CreateAIAgent();

        return new AgentCreationResult(agent, agentRunOptions);
    }

    /// <summary>
    ///     Builds an XAI agent and its run options from the specified configuration.
    /// </summary>
    /// <param name="connector">The agent connector.</param>
    /// <param name="configuration">The agent configuration.</param>
    /// <returns>The agent creation result containing the agent and run options.</returns>
    private static AgentCreationResult BuildxAIAgent(AgentConnector connector, AgentConfiguration configuration)
    {
        var client = new OpenAIClient(
            new ApiKeyCredential(connector.ApiKey),
            new OpenAIClientOptions
            {
                Endpoint = new Uri("https://api.x.ai/v1")
            });

        var agentRunOptions = new ChatClientAgentRunOptions(new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        });

        var agent = client.GetChatClient(connector.Model).CreateAIAgent(name: configuration.AgentName);

        return new AgentCreationResult(agent, agentRunOptions);
    }
}