// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using System.ClientModel;
using Amazon;
using Amazon.BedrockRuntime;
using Anthropic.SDK;
using Azure;
using Azure.AI.Agents.Persistent;
using Azure.AI.Inference;
using Azure.AI.OpenAI;
using Azure.Identity;
using GenerativeAI.Microsoft;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Mistral.SDK;
using OllamaSharp;
using OpenAI;
using SofDigital.Sempai.Core;
using SofDigital.Sempai.Core.Agents;
using SofDigital.Sempai.Core.Configurations;
using APIAuthentication = Anthropic.SDK.APIAuthentication;

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
                effectiveConfiguration.Tools ??= [];
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
                AgentProviderType.AwsBedrock => await BuildAwsBedrock(effectiveConnector, effectiveConfiguration),
                AgentProviderType.AzureOpenAI => BuildAzureOpenAIAgent(effectiveConnector, effectiveConfiguration),
                AgentProviderType.GithubModels => await BuildGithubModels(effectiveConnector, effectiveConfiguration),
                AgentProviderType.GoogleGemini => BuildGoogleGeminiAgent(effectiveConnector, effectiveConfiguration),
                AgentProviderType.Groq => BuildGrokAgent(effectiveConnector, effectiveConfiguration),
                AgentProviderType.Huggingface => BuildHuggingfaceAgent(effectiveConnector, effectiveConfiguration),
                AgentProviderType.Mistral => BuildMistralAgent(effectiveConnector, effectiveConfiguration),
                AgentProviderType.Ollama => BuildOllamaAgent(effectiveConnector, effectiveConfiguration),
                AgentProviderType.OpenAI => BuildOpenAIAgent(effectiveConnector, effectiveConfiguration),
                AgentProviderType.OpenRouter => BuildOpenRouterAgent(effectiveConnector, effectiveConfiguration),
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

    /// <inheritdoc />
    public AIFunction CreateFunction(Delegate method, string name, string description)
    {
        return AIFunctionFactory.Create(method, name, description);
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

        var agent = new ChatClientAgent(client,
            configuration?.Instructions,
            configuration?.AgentName);

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

        var chatOptions = new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        };

        var agentRunOptions = new ChatClientAgentRunOptions(chatOptions);

        var agent = await client.GetAIAgentAsync(aiFoundryAgent.Value.Id, chatOptions);

        return new AgentCreationResult(agent, agentRunOptions);
    }

    /// <summary>
    ///     Creates an AWS Bedrock-based agent along with its associated runtime options.
    /// </summary>
    /// <param name="connector">
    ///     An instance of <see cref="AgentConnector" /> that provides the API key and model details for the AWS Bedrock
    ///     integration.
    /// </param>
    /// <param name="configuration">
    ///     An instance of <see cref="AgentConfiguration" /> containing agent-specific settings, such as instructions, model
    ///     configuration, and tools.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation. The task result contains an <see cref="AgentCreationResult" />
    ///     that includes the constructed agent and its runtime options.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown when <paramref name="connector" /> or <paramref name="configuration" /> is null.
    /// </exception>
    private static async Task<AgentCreationResult> BuildAwsBedrock(AgentConnector connector,
        AgentConfiguration configuration)
    {
        Environment.SetEnvironmentVariable("AWS_BEARER_TOKEN_BEDROCK", connector.ApiKey);

        var regionName = connector.ResourceUri;
        var region = RegionEndpoint.GetBySystemName(regionName);
        
        var bedrockClient = new AmazonBedrockRuntimeClient(region);

        var chatOptions = new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        };

        var agentRunOptions = new ChatClientAgentRunOptions(chatOptions);

        var agent = new ChatClientAgent(bedrockClient.AsIChatClient(connector.Model),
            configuration?.Instructions,
            configuration?.AgentName,
            null,
            configuration?.Tools);

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

        var agent = chatClient.CreateAIAgent(
            configuration?.Instructions,
            configuration?.AgentName);

        return new AgentCreationResult(agent, agentRunOptions);
    }

    /// <summary>
    ///     Builds and configures GitHub-based AI models for use with agents.
    /// </summary>
    /// <param name="connector">
    ///     The <see cref="AgentConnector" /> containing connection details such as API key and model identifier.
    /// </param>
    /// <param name="configuration">
    ///     The <see cref="AgentConfiguration" /> specifying the configuration for the model, including instructions,
    ///     temperature, and tools.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="AgentCreationResult" /> containing the created chat client agent and any
    ///     associated run options.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="connector" /> or <paramref name="configuration" /> is null.
    /// </exception>
    private static async Task<AgentCreationResult> BuildGithubModels(AgentConnector connector,
        AgentConfiguration configuration)
    {
        var chatOptions = new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        };

        var agentRunOptions = new ChatClientAgentRunOptions(chatOptions);

        var agent = new ChatCompletionsClient(
                new Uri("https://models.github.ai/inference"),
                new AzureKeyCredential(connector.ApiKey),
                new AzureAIInferenceClientOptions())
            .AsIChatClient(connector.Model)
            .CreateAIAgent(
                configuration.Instructions,
                configuration.AgentName);

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

        var agent = new ChatClientAgent(client,
            configuration?.Instructions,
            configuration?.AgentName);

        return new AgentCreationResult(agent, agentRunOptions);
    }

    /// <summary>
    ///     Creates a Grok AI agent instance along with its run options based on the provided
    ///     connector and configuration details.
    /// </summary>
    /// <param name="connector">
    ///     The agent connector that contains API credentials and model information required
    ///     to initialize the Grok AI agent.
    /// </param>
    /// <param name="configuration">
    ///     The agent configuration that provides instructions, token limits, temperature settings,
    ///     and tool definitions for the agent's setup.
    /// </param>
    /// <returns>
    ///     An <see cref="AgentCreationResult" /> containing the created Grok AI agent and its
    ///     corresponding run options.
    /// </returns>
    private static AgentCreationResult BuildGrokAgent(AgentConnector connector, AgentConfiguration configuration)
    {
        var client = new OpenAIClient(
            new ApiKeyCredential(connector.ApiKey),
            new OpenAIClientOptions
            {
                Endpoint = new Uri("https://api.groq.com/openai/v1")
            });

        var agentRunOptions = new ChatClientAgentRunOptions(new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        });

        var agent = client.GetChatClient(connector.Model).CreateAIAgent(
            configuration?.Instructions,
            configuration?.AgentName);

        return new AgentCreationResult(agent, agentRunOptions);
    }

    /// <summary>
    ///     Builds an instance of a Huggingface-based AI agent and returns the associated creation result.
    /// </summary>
    /// <param name="connector">
    ///     The agent connector providing the API key and model information required for integration
    ///     with the Huggingface service.
    /// </param>
    /// <param name="configuration">
    ///     The agent configuration specifying parameters such as instructions, name, model behavior,
    ///     and other settings.
    /// </param>
    /// <returns>
    ///     An instance of <see cref="AgentCreationResult" /> containing the created agent and its
    ///     runtime options.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="connector" /> or <paramref name="configuration" /> is null.
    /// </exception>
    private static AgentCreationResult BuildHuggingfaceAgent(AgentConnector connector, AgentConfiguration configuration)
    {
        var client = new OpenAIClient(
            new ApiKeyCredential(connector.ApiKey),
            new OpenAIClientOptions
            {
                Endpoint = new Uri("https://router.huggingface.co/v1")
            });

        var agentRunOptions = new ChatClientAgentRunOptions(new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        });

        var agent = client.GetChatClient(connector.Model).CreateAIAgent(
            configuration?.Instructions,
            configuration?.AgentName);

        return new AgentCreationResult(agent, agentRunOptions);
    }

    /// <summary>
    ///     Builds and configures a Mistral agent instance using the provided connector and configuration.
    /// </summary>
    /// <param name="connector">
    ///     The <see cref="AgentConnector" /> containing API key and model details required
    ///     to authenticate and initialize the agent.
    /// </param>
    /// <param name="configuration">
    ///     The <see cref="AgentConfiguration" /> specifying options like instructions,
    ///     maximum output tokens, temperature, and tools for the agent.
    /// </param>
    /// <returns>
    ///     An <see cref="AgentCreationResult" /> containing the configured agent instance and
    ///     its optional runtime configuration parameters.
    /// </returns>
    private static AgentCreationResult BuildMistralAgent(AgentConnector connector, AgentConfiguration configuration)
    {
        var client = new MistralClient(new Mistral.SDK.APIAuthentication(connector.ApiKey));

        var chatOptions = new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        };

        var agentRunOptions = new ChatClientAgentRunOptions(chatOptions);

        var agent = client.Completions.CreateAIAgent(new ChatClientAgentOptions
        {
            ChatOptions = chatOptions
        });

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

        var agent = new ChatClientAgent(client,
            configuration?.Instructions,
            configuration?.AgentName);

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

        var agent = client.GetChatClient(connector.Model).CreateAIAgent(
            configuration?.Instructions,
            configuration?.AgentName);

        return new AgentCreationResult(agent, agentRunOptions);
    }

    /// <summary>
    ///     Builds and configures an OpenRouter-based AI agent using the provided agent connector and configuration.
    /// </summary>
    /// <param name="connector">The connector containing API key and model information required for the agent.</param>
    /// <param name="configuration">
    ///     The configuration details for the agent, including instructions, model specifications, and
    ///     other parameters.
    /// </param>
    /// <returns>
    ///     An <see cref="AgentCreationResult" /> containing the configured AI agent and its corresponding run options.
    /// </returns>
    /// <exception cref="ArgumentNullException">
    ///     Thrown if <paramref name="connector" /> or <paramref name="configuration" /> is null.
    /// </exception>
    private static AgentCreationResult BuildOpenRouterAgent(AgentConnector connector, AgentConfiguration configuration)
    {
        var client = new OpenAIClient(
            new ApiKeyCredential(connector.ApiKey),
            new OpenAIClientOptions
            {
                Endpoint = new Uri("https://openrouter.ai/api/v1")
            });

        var agentRunOptions = new ChatClientAgentRunOptions(new ChatOptions
        {
            Instructions = configuration.Instructions,
            MaxOutputTokens = configuration.MaxOutputTokens,
            ModelId = connector.Model,
            Temperature = configuration.Temperature ?? 0.7f,
            Tools = configuration.Tools
        });

        var agent = client.GetChatClient(connector.Model).CreateAIAgent(
            configuration?.Instructions,
            configuration?.AgentName);

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

        var agent = client.GetChatClient(connector.Model).CreateAIAgent(
            configuration?.Instructions,
            configuration?.AgentName);

        return new AgentCreationResult(agent, agentRunOptions);
    }
}