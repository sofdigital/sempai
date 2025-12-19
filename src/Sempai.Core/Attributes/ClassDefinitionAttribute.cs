// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace SofDigital.Sempai.Core.Attributes;

[AttributeUsage(AttributeTargets.Class, Inherited = false)]
public sealed class ClassDefinitionAttribute(string description) : Attribute
{
    public string Description { get; } = description;
}