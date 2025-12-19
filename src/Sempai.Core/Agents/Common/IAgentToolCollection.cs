// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Extensions.AI;

namespace SofDigital.Sempai.Core.Agents;

/// <summary>
///     Represents a collection of AI tools and provides methods to access tool information and collection metadata.
/// </summary>
/// <remarks>
///     Implementations of this interface allow retrieval of all tools in the collection, as well as access
///     to collection names. This interface is intended for scenarios where multiple tool collections may be managed or
///     queried.
/// </remarks>
public interface IAgentToolCollection
{
    /// <summary>
    ///     Gets or sets the name of the tool collection.
    /// </summary>
    /// <remarks>
    ///     This property represents the identifier or designation for a specific collection of AI tools.
    ///     It is used to distinguish between different tool collections within the system.
    /// </remarks>
    public string ToolCollectionName { get; set; }

    /// <summary>
    ///     Gets all tools in the collection.
    /// </summary>
    /// <returns></returns>
    IEnumerable<AITool> GetAllToolCollection();

    /// <summary>
    ///     Retrieves a collection of all tool collection names available in the system.
    /// </summary>
    /// <returns>
    ///     An enumerable sequence of strings containing the names of all tool collections. The sequence will be
    ///     empty if no tool collections are present.
    /// </returns>
    IEnumerable<string> GetAllToolCollectionNamesList();

    /// <summary>
    ///     Gets the name of the tool collection.
    /// </summary>
    /// <returns>The name of the tool collection.</returns>
    string GetToolCollectionName();
}