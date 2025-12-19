// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace SofDigital.Sempai.Core;

/// <summary>
///     Represents the tier of an agent model, indicating the level of features or capabilities available.
/// </summary>
/// <remarks>
///     The tiers define the capabilities and performance characteristics of the agent model:
///     <list
///         type="bullet">
///         <item>
///             <term>Basic</term>
///             <description>
///                 Provides essential functionality with limited
///                 features.
///             </description>
///         </item>
///         <item>
///             <term>Standard</term>
///             <description>
///                 Offers a balance of features and
///                 performance suitable for general use.
///             </description>
///         </item>
///         <item>
///             <term>Premium</term>
///             <description>
///                 Includes
///                 advanced features and higher performance for demanding scenarios.
///             </description>
///         </item>
///         <item>
///             <term>Reasoning</term>
///             <description>
///                 Specialized for complex reasoning tasks, providing the highest level of
///                 capability.
///             </description>
///         </item>
///     </list>
/// </remarks>
public enum AgentModelTierType
{
    Basic,
    Standard,
    Premium,
    Reasoning
}