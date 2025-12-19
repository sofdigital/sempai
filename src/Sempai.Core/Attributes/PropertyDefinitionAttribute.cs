// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace SofDigital.Sempai.Core.Attributes;

[AttributeUsage(AttributeTargets.Property)]
public sealed class PropertyDefinitionAttribute(string description, string? defaultValue = null) : Attribute
{
    public string Description { get; } = description;

    public string DefaultValue { get; set; } = defaultValue ?? "";
}