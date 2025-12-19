// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using System.Text.Json;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Provides a reusable abstract base implementation of <see cref="IAgent" /> that wraps a configured
///     <see cref="ChatClientAgent" /> instance and exposes unified execution methods (standard and streaming)
///     with multiple message overloads. Derived classes supply domain-specific tools via
///     <see
///         cref="GetTools" />
///     />.
/// </summary>
/// <remarks>
///     This base class centralizes:
///     <list type="bullet">
///         <item>
///             <description>Agent configuration assignment via <see cref="Configure" />.</description>
///         </item>
///         <item>
///             <description>Unified synchronous (task-based) and streaming run entry points.</description>
///         </item>
///         <item>
///             <description>Thread creation and deserialization helpers for conversation state.</description>
///         </item>
///     </list>
///     All execution methods validate that <see cref="ChatClientAgent" /> has been set and throw
///     <see cref="InvalidOperationException" /> otherwise.
/// </remarks>
public interface IAgent
{
    /// <summary>
    ///     Gets or sets the message thread associated with the agent's execution.
    /// </summary>
    AgentThread? AgentThread { get; set; }

    /// <summary>
    ///     Gets or sets the underlying configured <see cref="ChatClientAgent" /> used to execute runs. Must be
    ///     non-null prior to invoking any execution or thread operations.
    /// </summary>
    ChatClientAgent? ChatClientAgent { get; }

    /// <summary>
    ///     Gets or sets the configuration settings for the agent.
    /// </summary>
    public AgentConfiguration? AgentConfiguration { get; set; }

    /// <summary>
    ///     Gets or sets the name of the agent when used as a tool.
    /// </summary>
    public string? AgentAsToolName { get; set; }

    /// <summary>
    ///     Gets or sets the description of the agent when used as a tool.
    /// </summary>
    public string? AgentAsToolDescription { get; set; }

    /// <summary>
    ///     Gets or sets the default run options applied when an execution method is invoked without
    ///     explicit options.
    /// </summary>
    ChatClientAgentRunOptions AgentRunOptions { get; set; }

    /// <summary>
    ///     Configures the agent instance with a backing <see cref="ChatClientAgent" />, its
    ///     <see cref="Agents.AgentConfiguration" />, and optional default <see cref="Microsoft.Agents.AI.AgentRunOptions" />.
    /// </summary>
    /// <param name="configuration">The logical configuration metadata for this agent.</param>
    void Configure(AgentConfiguration configuration);

    /// <summary>
    ///     Establishes a connection between the agent and the specified <see cref="AgentConnector" />,
    ///     enabling communication with the underlying AI provider or service.
    /// </summary>
    /// <param name="connector">The connector containing provider, model, and authentication details.</param>
    void Connect(AgentConnector connector);

    /// <summary>
    ///     Reconstructs an <see cref="AgentThread" /> from serialized JSON.
    /// </summary>
    /// <param name="serializedThread">The JSON element representing the thread.</param>
    /// <param name="jsonSerializerOptions">Optional serializer customization.</param>
    /// <returns>The deserialized <see cref="AgentThread" /> instance.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="ChatClientAgent" /> is not configured.
    /// </exception>
    AgentThread DeserializeThread(JsonElement serializedThread, JsonSerializerOptions? jsonSerializerOptions = null);

    /// <summary>
    ///     Returns an <see cref="AIFunction" /> representation of the agent, allowing it to be used as a tool
    ///     within other agent contexts or toolchains.
    /// </summary>
    /// <returns>
    ///     An <see cref="AIFunction" /> instance that encapsulates the agent's capabilities and metadata,
    ///     or <c>null</c> if the agent cannot be represented as a tool.
    /// </returns>
    AIFunction? GetAsAgentTool();

    /// <summary>
    ///     Creates a new <see cref="AgentThread" /> for maintaining conversational or task state.
    /// </summary>
    /// <returns>A newly created <see cref="AgentThread" />.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="ChatClientAgent" /> is not configured.
    /// </exception>
    AgentThread GetNewThread();

    /// <summary>
    ///     Returns the concrete tools (capabilities) this agent exposes in addition to global or shared tools.
    /// </summary>
    /// <returns>A collection of <see cref="AITool" /> instances.</returns>
    IEnumerable<AITool>? GetTools();

    /// <summary>
    ///     Initializes the specified AI agent with the provided run options.
    /// </summary>
    /// <param name="agent">The AI agent to be initialized. Cannot be null.</param>
    /// <param name="options">
    ///     Optional configuration settings that control the agent's initialization behavior.
    ///     If null, default options are used.
    /// </param>
    void Initialize(ChatClientAgent agent, ChatClientAgentRunOptions? options);

    /// <summary>
    ///     Executes the agent using an existing or new thread without adding additional messages
    ///     (continue scenario).
    /// </summary>
    /// <param name="thread">Optional existing thread; if null the underlying agent may start a new context.</param>
    /// <param name="options">Optional run overrides; if null <see cref="AgentRunOptions" /> is used.</param>
    /// <param name="cancellationToken">Token to observe cancellation.</param>
    /// <returns>The final <see cref="AgentRunResponse" />.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="ChatClientAgent" /> is not configured.
    /// </exception>
    Task<AgentRunResponse> RunAsync(
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes the agent with a single string message (e.g., user prompt) and optional thread/context.
    /// </summary>
    /// <param name="message">The textual message to append and process.</param>
    /// <param name="thread">Optional existing thread context.</param>
    /// <param name="options">Optional run overrides; if null <see cref="AgentRunOptions" /> is used.</param>
    /// <param name="cancellationToken">Token to observe cancellation.</param>
    /// <returns>The final <see cref="AgentRunResponse" />.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="ChatClientAgent" /> is not configured.
    /// </exception>
    Task<AgentRunResponse> RunAsync(
        string message,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes the agent with a single structured <see cref="ChatMessage" /> (supports roles,
    ///     metadata, etc.).
    /// </summary>
    /// <param name="message">The structured chat message to process.</param>
    /// <param name="thread">Optional existing thread context.</param>
    /// <param name="options">Optional run overrides; if null <see cref="AgentRunOptions" /> is used.</param>
    /// <param name="cancellationToken">Token to observe cancellation.</param>
    /// <returns>The final <see cref="AgentRunResponse" />.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="ChatClientAgent" /> is not configured.
    /// </exception>
    Task<AgentRunResponse> RunAsync(
        ChatMessage message,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes the agent with a batch (ordered collection) of chat messages.
    /// </summary>
    /// <param name="messages">The collection of messages appended to the thread.</param>
    /// <param name="thread">Optional existing thread context.</param>
    /// <param name="options">Optional run overrides; if null <see cref="AgentRunOptions" /> is used.</param>
    /// <param name="cancellationToken">Token to observe cancellation.</param>
    /// <returns>The final <see cref="AgentRunResponse" />.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="ChatClientAgent" /> is not configured.
    /// </exception>
    Task<AgentRunResponse> RunAsync(
        IEnumerable<ChatMessage> messages,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes the agent with a strongly-typed response, using an existing or new thread and optional serialization and
    ///     run options.
    /// </summary>
    /// <typeparam name="T">The type to which the agent's response will be deserialized.</typeparam>
    /// <param name="thread">Optional existing thread context; if null, a new context may be started.</param>
    /// <param name="serializerOptions">Optional JSON serializer settings for response deserialization.</param>
    /// <param name="options">Optional run overrides; if null, <see cref="AgentRunOptions" /> is used.</param>
    /// <param name="useJsonSchemaResponseFormat">If true, enforces JSON schema response formatting.</param>
    /// <param name="cancellationToken">Token to observe cancellation.</param>
    /// <returns>The final <see cref="ChatClientAgentRunResponse{T}" /> containing the deserialized result.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ChatClientAgent" /> is not configured.</exception>
    Task<ChatClientAgentRunResponse<T>> RunAsync<T>(
        AgentThread? thread = null,
        JsonSerializerOptions? serializerOptions = null,
        AgentRunOptions? options = null,
        bool? useJsonSchemaResponseFormat = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes the agent with a strongly-typed response, using a string message and optional thread, serialization, and
    ///     run options.
    /// </summary>
    /// <typeparam name="T">The type to which the agent's response will be deserialized.</typeparam>
    /// <param name="message">The textual message to process.</param>
    /// <param name="thread">Optional existing thread context.</param>
    /// <param name="serializerOptions">Optional JSON serializer settings for response deserialization.</param>
    /// <param name="options">Optional run overrides; if null, <see cref="AgentRunOptions" /> is used.</param>
    /// <param name="useJsonSchemaResponseFormat">If true, enforces JSON schema response formatting.</param>
    /// <param name="cancellationToken">Token to observe cancellation.</param>
    /// <returns>The final <see cref="ChatClientAgentRunResponse{T}" /> containing the deserialized result.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ChatClientAgent" /> is not configured.</exception>
    Task<ChatClientAgentRunResponse<T>> RunAsync<T>(
        string message,
        AgentThread? thread = null,
        JsonSerializerOptions? serializerOptions = null,
        AgentRunOptions? options = null,
        bool? useJsonSchemaResponseFormat = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes the agent with a strongly-typed response, using a structured <see cref="ChatMessage" /> and optional
    ///     thread, serialization, and run options.
    /// </summary>
    /// <typeparam name="T">The type to which the agent's response will be deserialized.</typeparam>
    /// <param name="message">The structured chat message to process.</param>
    /// <param name="thread">Optional existing thread context.</param>
    /// <param name="serializerOptions">Optional JSON serializer settings for response deserialization.</param>
    /// <param name="options">Optional run overrides; if null, <see cref="AgentRunOptions" /> is used.</param>
    /// <param name="useJsonSchemaResponseFormat">If true, enforces JSON schema response formatting.</param>
    /// <param name="cancellationToken">Token to observe cancellation.</param>
    /// <returns>The final <see cref="ChatClientAgentRunResponse{T}" /> containing the deserialized result.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ChatClientAgent" /> is not configured.</exception>
    Task<ChatClientAgentRunResponse<T>> RunAsync<T>(
        ChatMessage message,
        AgentThread? thread = null,
        JsonSerializerOptions? serializerOptions = null,
        AgentRunOptions? options = null,
        bool? useJsonSchemaResponseFormat = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes the agent with a strongly-typed response, using a batch of structured messages and optional thread,
    ///     serialization, and run options.
    /// </summary>
    /// <typeparam name="T">The type to which the agent's response will be deserialized.</typeparam>
    /// <param name="messages">The collection of messages to process.</param>
    /// <param name="thread">Optional existing thread context.</param>
    /// <param name="serializerOptions">Optional JSON serializer settings for response deserialization.</param>
    /// <param name="options">Optional run overrides; if null, <see cref="AgentRunOptions" /> is used.</param>
    /// <param name="useJsonSchemaResponseFormat">If true, enforces JSON schema response formatting.</param>
    /// <param name="cancellationToken">Token to observe cancellation.</param>
    /// <returns>The final <see cref="ChatClientAgentRunResponse{T}" /> containing the deserialized result.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="ChatClientAgent" /> is not configured.</exception>
    Task<ChatClientAgentRunResponse<T>> RunAsync<T>(
        IEnumerable<ChatMessage> messages,
        AgentThread? thread = null,
        JsonSerializerOptions? serializerOptions = null,
        AgentRunOptions? options = null,
        bool? useJsonSchemaResponseFormat = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes the agent in streaming mode with an existing or new thread.
    /// </summary>
    /// <param name="thread">Optional existing thread context.</param>
    /// <param name="options">Optional run overrides; if null <see cref="AgentRunOptions" /> is used.</param>
    /// <param name="cancellationToken">Token to observe cancellation.</param>
    /// <returns>An async sequence of <see cref="AgentRunResponseUpdate" /> items.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="ChatClientAgent" /> is not configured.
    /// </exception>
    IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes the agent in streaming mode with a single raw string message.
    /// </summary>
    /// <param name="message">The textual message to process.</param>
    /// <param name="thread">Optional existing thread context.</param>
    /// <param name="options">Optional run overrides; if null <see cref="AgentRunOptions" /> is used.</param>
    /// <param name="cancellationToken">Token to observe cancellation.</param>
    /// <returns>An async sequence of <see cref="AgentRunResponseUpdate" /> items.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="ChatClientAgent" /> is not configured.
    /// </exception>
    IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(
        string message,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes the agent in streaming mode with a single structured <see cref="ChatMessage" />.
    /// </summary>
    /// <param name="message">The structured message to process.</param>
    /// <param name="thread">Optional existing thread context.</param>
    /// <param name="options">Optional run overrides; if null <see cref="AgentRunOptions" /> is used.</param>
    /// <param name="cancellationToken">Token to observe cancellation.</param>
    /// <returns>An async sequence of <see cref="AgentRunResponseUpdate" /> items.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="ChatClientAgent" /> is not configured.
    /// </exception>
    IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(
        ChatMessage message,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    ///     Executes the agent in streaming mode with a batch of structured messages.
    /// </summary>
    /// <param name="messages">Ordered collection of messages to append and process.</param>
    /// <param name="thread">Optional existing thread context.</param>
    /// <param name="options">Optional run overrides; if null <see cref="AgentRunOptions" /> is used.</param>
    /// <param name="cancellationToken">Token to observe cancellation.</param>
    /// <returns>An async sequence of <see cref="AgentRunResponseUpdate" /> items.</returns>
    /// <exception cref="InvalidOperationException">
    ///     Thrown when <see cref="ChatClientAgent" /> is not configured.
    /// </exception>
    IAsyncEnumerable<AgentRunResponseUpdate> RunStreamingAsync(
        IEnumerable<ChatMessage> messages,
        AgentThread? thread = null,
        AgentRunOptions? options = null,
        CancellationToken cancellationToken = default);
}