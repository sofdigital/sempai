// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Extensions.AI;

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Defines the contract for creating various types of agent messages, such as text, file, and image messages.
/// </summary>
public interface IAgentMessageFactory
{
    /// <summary>
    ///     Creates a <see cref="ChatMessage" /> containing a text prompt and a file as binary data.
    /// </summary>
    /// <param name="prompt">The text prompt to include in the message.</param>
    /// <param name="fileData">The binary data of the file.</param>
    /// <param name="mimeType">The MIME type of the file. Defaults to "application/octet-stream".</param>
    /// <returns>A <see cref="ChatMessage" /> with text and file data content.</returns>
    public ChatMessage CreateFileMessage(string prompt, byte[] fileData, string mimeType = "application/octet-stream");

    /// <summary>
    ///     Creates a <see cref="ChatMessage" /> containing a text prompt and an image referenced by URI.
    /// </summary>
    /// <param name="prompt">The text prompt to include in the message.</param>
    /// <param name="uri">The URI of the image.</param>
    /// <param name="mimeType">The MIME type of the image. Defaults to "image/jpg".</param>
    /// <returns>A <see cref="ChatMessage" /> with text and image URI content.</returns>
    public ChatMessage CreateImageMessage(string prompt, string uri, string mimeType = "image/jpg");

    /// <summary>
    ///     Creates a <see cref="ChatMessage" /> containing a text prompt and an image as binary data.
    /// </summary>
    /// <param name="prompt">The text prompt to include in the message.</param>
    /// <param name="imageData">The binary data of the image.</param>
    /// <param name="mimeType">The MIME type of the image. Defaults to "image/jpg".</param>
    /// <returns>A <see cref="ChatMessage" /> with text and image data content.</returns>
    public ChatMessage CreateImageMessage(string prompt, byte[] imageData, string mimeType = "image/jpg");

    // <summary>
    /// Creates a structured instruction string for use in chat messages.
    /// </summary>
    /// <param name="instructions">The instructions or task description to include.</param>
    /// <param name="role">The role for which the instructions are intended (e.g., "user", "assistant").</param>
    /// <param name="temperament">The temperament of the agent</param>
    /// <param name="output">The expected output or result (currently unused).</param>
    /// <param name="rules">Rules or guidelines (currently unused).</param>
    /// <param name="knowledgeCutoff">
    ///     The date representing the knowledge cutoff for the instructions. If null, defaults to one
    ///     year before the current date.
    /// </param>
    /// <returns>
    ///     A formatted string containing the role, current date, knowledge cutoff, and instructions/task.
    /// </returns>
    public string CreateStructuredInstructions(
        string instructions,
        string? role = null,
        string? temperament = null,
        string? rules = null,
        string? output = null,
        DateTime? knowledgeCutoff = null);

    /// <summary>
    ///     Creates a <see cref="ChatMessage" /> containing only a text prompt.
    /// </summary>
    /// <param name="prompt">The text prompt to include in the message.</param>
    /// <returns>A <see cref="ChatMessage" /> with text content.</returns>
    public ChatMessage CreateTextMessage(string prompt);
}