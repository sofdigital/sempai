// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace SofDigital.Sempai.Core.Configurations;

/// <summary>
///     Defines a contract for wrapping configuration parameters.
/// </summary>
public interface IConfigurationWrapper
{
    object ParametersObject { get; }
}