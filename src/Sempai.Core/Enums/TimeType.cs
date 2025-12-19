// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace SofDigital.Sempai.Core;

/// <summary>
///     Represents the type of time standard.
/// </summary>
/// <remarks>
///     This enumeration defines the two commonly used time standards:
///     Local time and Coordinated Universal Time (UTC).
/// </remarks>
public enum TimeType
{
    /// <summary>
    ///     Represents the local time standard.
    /// </summary>
    /// <remarks>
    ///     Local refers to the time standard that is based on the system's configured timezone.
    ///     It accounts for timezone offsets and may also include adjustments for daylight saving time.
    /// </remarks>
    Local,

    /// <summary>
    ///     Represents the Coordinated Universal Time (UTC) standard.
    /// </summary>
    /// <remarks>
    ///     UTC is the primary time standard by which the world regulates clocks and time.
    ///     It is not affected by daylight saving time and does not include timezone offsets.
    /// </remarks>
    Utc
}