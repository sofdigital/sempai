// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

namespace Sempai.Tests.Utilities;

public static class ConsoleUtilities
{
    public static void PrintUserMessage(string message)
    {
        Console.WriteLine();

        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Red;
        Console.WriteLine("User: " + message);
        Console.ForegroundColor = previousColor;

        Console.WriteLine();
    }

    public static void PrintAgentMessage(string agentName, string message)
    {
        Console.WriteLine();

        var previousColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Blue;
        Console.WriteLine($"{agentName}: " + message);
        Console.ForegroundColor = previousColor;

        Console.WriteLine();
    }
}