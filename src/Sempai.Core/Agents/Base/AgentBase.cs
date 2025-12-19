// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace SofDigital.Sempai.Core.Agents;

/// <inheritdoc />
public abstract class AgentBase
    : IAgent
{
    /// <summary>
    ///     Initializes a new unconfigured instance of <see cref="AgentBase" />.
    ///     A subsequent call to <see cref="Configure" /> is required before execution.
    /// </summary>
    protected AgentBase()
    {
        AgentRunOptions = new ChatClientAgentRunOptions();
    }

    /// <summary>
    ///     Initializes a new instance of <see cref="AgentBase" /> with a backing <see cref="ChatClientAgent" />
    ///     and optional default run options.
    /// </summary>
    /// <param name="agent">The underlying <see cref="ChatClientAgent" /> used to perform executions.</param>
    /// <param name="options">
    ///     Optional default <see cref="Microsoft.Agents.AI.AgentRunOptions" /> applied when
    ///     per-call options are not supplied.
    /// </param>
    /// <exception cref="ArgumentNullException">Thrown when <paramref name="agent" /> is <c>null</c>.</exception>
    protected AgentBase(ChatClientAgent agent, ChatClientAgentRunOptions? options)
    {
        ChatClientAgent = agent ?? throw new ArgumentNullException(nameof(agent));
        AgentRunOptions = options ?? new ChatClientAgentRunOptions();
    }

    /// <summary>
    ///     Gets or sets the connector used to interact with the agent.
    /// </summary>
    public AgentConnector? AgentConnector { get; set; }

    /// <inheritdoc />
    public AgentThread? AgentThread { get; set; }

    /// <inheritdoc />
    public ChatClientAgent? ChatClientAgent { get; set; }

    /// <inheritdoc />
    public AgentConfiguration? AgentConfiguration { get; set; } = new();

    /// <inheritdoc />
    public string? AgentAsToolName { get; set; }

    /// <inheritdoc />
    public string? AgentAsToolDescription { get; set; }

    /// <inheritdoc />
    public ChatClientAgentRunOptions AgentRunOptions { get; set; }

    /// <inheritdoc />
    public void Configure(AgentConfiguration configuration)
    {
        AgentConfiguration = configuration;
    }

    /// <inheritdoc />
    public void Connect(AgentConnector connector)
    {
        AgentConnector = connector;
    }

    /// <inheritdoc />
    public virtual AgentThread DeserializeThread(JsonElement serializedThread,
        JsonSerializerOptions? jsonSerializerOptions = null)
    {
        return ChatClientAgent == null
            ? throw new InvalidOperationException("ChatClientAgent is not configured.")
            : ChatClientAgent.DeserializeThread(serializedThread, jsonSerializerOptions);
    }

    /// <inheritdoc />
    public virtual AIFunction? GetAsAgentTool()
    {
        return ChatClientAgent!.AsAIFunction(new AIFunctionFactoryOptions
        {
            Name = AgentAsToolName ?? "Agent",
            Description = AgentAsToolDescription ?? "An AI agent with specialized tools and capabilities."
        });
    }

    /// <inheritdoc />
    public virtual AgentThread GetNewThread()
    {
        return ChatClientAgent == null
            ? throw new InvalidOperationException("ChatClientAgent is not configured.")
            : ChatClientAgent.GetNewThread();
    }

    /// <inheritdoc />
    public abstract IEnumerable<AITool>? GetTools();

    /// <inheritdoc />
    public void Initialize(ChatClientAgent agent, ChatClientAgentRunOptions? options)
    {
        ChatClientAgent = agent;
        AgentRunOptions = options!;

        if (AgentConfiguration is { Threaded: true }) AgentThread = GetNewThread();
    }

    /// <inheritdoc />
    public virtual Task<AgentRunResponse> RunAsync(
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        return ChatClientAgent.RunAsync(runThread, runOptions, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<AgentRunResponse> RunAsync(
        string message,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        return ChatClientAgent.RunAsync(message, runThread, runOptions, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<AgentRunResponse> RunAsync(
        ChatMessage message,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runOptions = options ?? AgentRunOptions;
        var runThread = thread ?? AgentThread;

        return ChatClientAgent.RunAsync(message, runThread, runOptions, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<AgentRunResponse> RunAsync(
        IEnumerable<ChatMessage> messages,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        return ChatClientAgent.RunAsync(messages, runThread, runOptions, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<ChatClientAgentRunResponse<T>> RunAsync<T>(
        AgentThread? thread = null,
        JsonSerializerOptions? serializerOptions = null,
        AgentRunOptions? options = null,
        bool? useJsonSchemaResponseFormat = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        return ChatClientAgent.RunAsync<T>(runThread, serializerOptions, runOptions,
            useJsonSchemaResponseFormat, CancellationToken.None);
    }

    /// <inheritdoc />
    public virtual Task<ChatClientAgentRunResponse<T>> RunAsync<T>(
        string message,
        AgentThread? thread = null,
        JsonSerializerOptions? serializerOptions = null,
        AgentRunOptions? options = null,
        bool? useJsonSchemaResponseFormat = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        return ChatClientAgent.RunAsync<T>(message, runThread, serializerOptions,
            runOptions, useJsonSchemaResponseFormat, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<ChatClientAgentRunResponse<T>> RunAsync<T>(
        ChatMessage message,
        AgentThread? thread = null,
        JsonSerializerOptions? serializerOptions = null,
        AgentRunOptions? options = null,
        bool? useJsonSchemaResponseFormat = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        return ChatClientAgent.RunAsync<T>(message, runThread, serializerOptions,
            runOptions, useJsonSchemaResponseFormat, cancellationToken);
    }

    /// <inheritdoc />
    public virtual Task<ChatClientAgentRunResponse<T>> RunAsync<T>(
        IEnumerable<ChatMessage> messages,
        AgentThread? thread = null,
        JsonSerializerOptions? serializerOptions = null,
        AgentRunOptions? options = null,
        bool? useJsonSchemaResponseFormat = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        return ChatClientAgent.RunAsync<T>(messages, runThread, serializerOptions,
            runOptions, useJsonSchemaResponseFormat, cancellationToken);
    }

    /// <inheritdoc />
    public virtual IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        return ChatClientAgent.RunStreamingAsync(runThread, runOptions, cancellationToken);
    }

    /// <inheritdoc />
    public virtual IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(
        string message,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        return ChatClientAgent.RunStreamingAsync(message, runThread, runOptions, cancellationToken);
    }

    /// <inheritdoc />
    public virtual IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(
        ChatMessage message,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        return ChatClientAgent.RunStreamingAsync(message, runThread, runOptions, cancellationToken);
    }

    /// <inheritdoc />
    public virtual IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(
        IEnumerable<ChatMessage> messages,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        return ChatClientAgent.RunStreamingAsync(messages, runThread, runOptions, cancellationToken);
    }
}