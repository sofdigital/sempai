// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using System.Text;
using Microsoft.Extensions.AI;
using SofDigital.Sempai.Core.Agents;

namespace SofDigital.Sempai.Agents.Factories;

/// <inheritdoc />
public class AgentMessageFactory : IAgentMessageFactory
{
    /// <inheritdoc />
    public string CreateStructuredInstructions(string instructions, string? role = null, string? temperament = null,
        string? rules = null, string? output = null, DateTime? knowledgeCutoff = null)
    {
        var sb = new StringBuilder();

        if (!string.IsNullOrEmpty(role)) sb.Append("** Role ** : " + role);

        sb.Append("** Current Date ** : " + DateTime.Now.ToString("yyyy-MM-dd"));

        if (knowledgeCutoff != null)
            sb.Append("** Knowledge Cutoff ** : " + knowledgeCutoff.Value.ToString("yyyy-MM-dd"));

        sb.Append("** Instructions ** : " + instructions);

        if (!string.IsNullOrEmpty(temperament)) sb.Append("** Temperament ** : " + role);

        if (!string.IsNullOrEmpty(rules)) sb.Append("** Rules ** : " + rules);

        if (!string.IsNullOrEmpty(output)) sb.Append("** Output ** : " + output);

        sb.AppendLine();

        return sb.ToString();
    }

    /// <inheritdoc />
    public ChatMessage CreateFileMessage(string prompt, byte[] fileData, string mimeType = "application/octet-stream")
    {
        return new ChatMessage(ChatRole.User,
        [
            new TextContent(prompt),
            new DataContent(fileData, mimeType)
        ]);
    }

    /// <inheritdoc />
    public ChatMessage CreateImageMessage(string prompt, string uri, string mimeType = "image/jpg")
    {
        return new ChatMessage(ChatRole.User,
        [
            new TextContent(prompt),
            new UriContent(uri, mimeType)
        ]);
    }

    /// <inheritdoc />
    public ChatMessage CreateImageMessage(string prompt, byte[] imageData, string mimeType = "image/jpg")
    {
        return new ChatMessage(ChatRole.User,
        [
            new TextContent(prompt),
            new DataContent(imageData, mimeType)
        ]);
    }

    /// <inheritdoc />
    public ChatMessage CreateTextMessage(string prompt)
    {
        return new ChatMessage(ChatRole.User, [new TextContent(prompt)]);
    }
}