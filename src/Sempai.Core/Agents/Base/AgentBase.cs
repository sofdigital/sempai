// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using System.Runtime.CompilerServices;
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
    public event Action<AgentTokenUsageResult>? OnTokenUsageUpdated;

    /// <inheritdoc />
    public long TokenCountInput { get; set; }

    /// <inheritdoc />
    public long TokenCountOutput { get; set; }

    /// <inheritdoc />
    public long TokenCountReasoning { get; set; }

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
    public virtual async Task<AgentRunResponse> RunAsync(
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        var response = await ChatClientAgent.RunAsync(runThread, runOptions, cancellationToken);

        OnTokenUsageAvailable(response.Usage);

        return response;
    }

    /// <inheritdoc />
    public virtual async Task<AgentRunResponse> RunAsync(
        string message,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        var response = await ChatClientAgent.RunAsync(message, runThread, runOptions, cancellationToken);

        OnTokenUsageAvailable(response.Usage);

        return response;
    }

    /// <inheritdoc />
    public virtual async Task<AgentRunResponse> RunAsync(
        ChatMessage message,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runOptions = options ?? AgentRunOptions;
        var runThread = thread ?? AgentThread;

        var response = await ChatClientAgent.RunAsync(message, runThread, runOptions, cancellationToken);

        return await RunWithUsageAsync(response);
    }

    /// <inheritdoc />
    public virtual async Task<AgentRunResponse> RunAsync(
        IEnumerable<ChatMessage> messages,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        var response = await ChatClientAgent.RunAsync(messages, runThread, runOptions, cancellationToken);

        return await RunWithUsageAsync(response);
    }

    /// <inheritdoc />
    public virtual async Task<AgentRunResponse<T>> RunAsync<T>(
        AgentThread? thread = null,
        JsonSerializerOptions? serializerOptions = null,
        AgentRunOptions? options = null,
        bool? useJsonSchemaResponseFormat = null,
        CancellationToken cancellationToken = default)
    {
        if (ChatClientAgent == null) throw new InvalidOperationException("ChatClientAgent is not configured.");

        var runThread = thread ?? AgentThread;
        var runOptions = options ?? AgentRunOptions;

        var response = await ChatClientAgent.RunAsync<T>(runThread, serializerOptions, runOptions,
            useJsonSchemaResponseFormat, CancellationToken.None);

        return await RunWithUsageAsync(response);
    }

    /// <inheritdoc />
    public virtual async Task<AgentRunResponse<T>> RunAsync<T>(
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

        var response = await ChatClientAgent.RunAsync<T>(message, runThread, serializerOptions,
            runOptions, useJsonSchemaResponseFormat, cancellationToken);

        return await RunWithUsageAsync(response);
    }

    /// <inheritdoc />
    public virtual async Task<AgentRunResponse<T>> RunAsync<T>(
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

        var response = await ChatClientAgent.RunAsync<T>(message, runThread, serializerOptions,
            runOptions, useJsonSchemaResponseFormat, cancellationToken);

        return await RunWithUsageAsync(response);
    }

    /// <inheritdoc />
    public virtual async Task<AgentRunResponse<T>> RunAsync<T>(
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

        var response = await ChatClientAgent.RunAsync<T>(messages, runThread, serializerOptions,
            runOptions, useJsonSchemaResponseFormat, cancellationToken);

        return await RunWithUsageAsync(response);
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

        var stream = ChatClientAgent.RunStreamingAsync(runThread, runOptions, cancellationToken);

        return RunStreamingWithUsageAsync(stream, cancellationToken);
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

        var stream = ChatClientAgent.RunStreamingAsync(message, runThread, runOptions, cancellationToken);

        return RunStreamingWithUsageAsync(stream, cancellationToken);
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

        var stream = ChatClientAgent.RunStreamingAsync(message, runThread, runOptions, cancellationToken);

        return RunStreamingWithUsageAsync(stream, cancellationToken);
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

        var stream = ChatClientAgent.RunStreamingAsync(messages, runThread, runOptions, cancellationToken);

        return RunStreamingWithUsageAsync(stream, cancellationToken);
    }

    /// <summary>
    ///     Updates token usage details based on the provided usage information and triggers the
    ///     <see cref="OnTokenUsageUpdated" /> event.
    /// </summary>
    /// <param name="result">
    ///     The usage details containing input, output, and any additional token count information.
    ///     If <paramref name="result" /> is null, no action is taken.
    /// </param>
    private void OnTokenUsageAvailable(UsageDetails? result)
    {
        if (result is null) return;

        var inputTokenCount = result.InputTokenCount ?? 0;
        var outputTokenCount = result.OutputTokenCount ?? 0;
        long reasoningCount = 0;

        var reasoning = result
            .AdditionalCounts?.TryGetValue("OutputTokenDetails.ReasoningTokenCount", out reasoningCount);

        TokenCountInput += inputTokenCount;
        TokenCountOutput += outputTokenCount;

        if (reasoning.HasValue && reasoning.Value) TokenCountReasoning += reasoningCount!;

        var usage = new AgentTokenUsageResult(
            inputTokenCount,
            outputTokenCount,
            reasoningCount,
            TokenCountInput,
            TokenCountOutput,
            TokenCountReasoning);

        OnTokenUsageUpdated?.Invoke(usage);
    }

    private async IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingWithUsageAsync(
        IAsyncEnumerable<AgentRunResponseUpdate> stream,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var updates = new List<AgentRunResponseUpdate>();

        await foreach (var update in stream.WithCancellation(cancellationToken).ConfigureAwait(false))
        {
            updates.Add(update);
            yield return update;
        }

        if (updates.Count <= 0) yield break;

        var response = await ToAsyncEnumerable(updates)
            .ToAgentRunResponseAsync(cancellationToken)
            .ConfigureAwait(false);

        OnTokenUsageAvailable(response.Usage);
    }

    /// <summary>
    ///     Updates the token usage details based on the provided response and returns the response asynchronously.
    /// </summary>
    /// <param name="response">
    ///     The agent response containing the token usage data to be tracked.
    /// </param>
    /// <returns>
    ///     A task that represents the asynchronous operation, returning the original <see cref="AgentRunResponse" />.
    /// </returns>
    private Task<AgentRunResponse> RunWithUsageAsync(AgentRunResponse response)
    {
        OnTokenUsageAvailable(response.Usage);

        return Task.FromResult(response);
    }

    /// <summary>
    ///     Processes the provided <paramref name="response" /> to collect usage details
    ///     and returns the processed response.
    /// </summary>
    /// <typeparam name="T">The type of the response content.</typeparam>
    /// <param name="response">The response to be processed and returned.</param>
    /// <returns>
    ///     A task representing the asynchronous operation, with the resulting
    ///     <see cref="AgentRunResponse{T}" /> containing the processed response.
    /// </returns>
    private Task<AgentRunResponse<T>> RunWithUsageAsync<T>(AgentRunResponse<T> response)
    {
        OnTokenUsageAvailable(response.Usage);

        return Task.FromResult(response);
    }

    /// <summary>
    ///     Converts a specified <see cref="IEnumerable{T}" /> to an <see cref="IAsyncEnumerable{T}" />.
    /// </summary>
    /// <typeparam name="T">The type of elements in the enumerable.</typeparam>
    /// <param name="items">The collection of items to be converted to an asynchronous enumerable.</param>
    /// <returns>An <see cref="IAsyncEnumerable{T}" /> that represents the asynchronous sequence of the provided items.</returns>
    private static async IAsyncEnumerable<T> ToAsyncEnumerable<T>(IEnumerable<T> items)
    {
        await Task.Yield();

        foreach (var item in items)
            yield return item;
    }
}