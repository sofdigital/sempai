// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using FluentAssertions;
using Microsoft.Extensions.Logging;
using Sempai.Tests.Features.Fixtures;
using Sempai.Tests.Utilities;
using SofDigital.Sempai.Agents;
using SofDigital.Sempai.Core.Agents;
using Xunit;

namespace Sempai.Tests.Features.Tests;

public class AgentTests(AgentTestsFixture fixture) : IClassFixture<AgentTestsFixture>
{
    [Fact]
    public async Task AgentTests_TokenUsageTest()
    {
        fixture.Logger.LogInformation("Starting AgentTests_TokenUsageTest test");

        var defaultConnector = fixture.GetAgentDefaultConnector();

        var configuration = new AgentConfiguration
        {
            AgentName = "TimeAgent",
            Autonomous = false,
            Instructions = "You are a helpful assistant.",
            MaxOutputTokens = 100,
            Temperature = 0.7f,
            Threaded = false,
            Tools = [],
            WebSearchEnabled = false
        };

        const string prompt = "What time is the current time in UTC?";

        ConsoleUtilities.PrintUserMessage(prompt);

        var agent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, configuration);
        var message = fixture.AgentMessageFactory.CreateTextMessage(prompt);
        var response = await agent!.RunAsync(message);

        ConsoleUtilities.PrintAgentMessage(configuration.AgentName, response.Text);

        response.Should().NotBeNull();

        agent.TokenCountInput.Should().BeGreaterThan(0);
        agent.TokenCountOutput.Should().BeGreaterThan(0);

        fixture.Logger.LogInformation("Finished AgentTests_TokenUsageTest test");
    }

    [Fact]
    public async Task AgentTests_TokenUsageStreamTest()
    {
        fixture.Logger.LogInformation("Starting AgentTests_TokenUsageStreamTest test");

        var defaultConnector = fixture.GetAgentDefaultConnector();

        var configuration = new AgentConfiguration
        {
            AgentName = "TimeAgent",
            Autonomous = false,
            Instructions = "You are a helpful assistant.",
            MaxOutputTokens = 100,
            Temperature = 0.7f,
            Threaded = true,
            Tools = [],
            WebSearchEnabled = false
        };

        const string prompt = "What time is the current time in UTC?";

        ConsoleUtilities.PrintUserMessage(prompt);

        var agent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, configuration);
        agent.AgentThread = agent.GetNewThread();

        var message = fixture.AgentMessageFactory.CreateTextMessage(prompt);
        var stream = agent!.RunStreamingAsync(message);

        await foreach (var response in stream)
            ConsoleUtilities.PrintAgentMessage(configuration.AgentName, response.Text);

        stream.Should().NotBeNull();
        agent.TokenCountInput.Should().BeGreaterThan(0);
        agent.TokenCountOutput.Should().BeGreaterThan(0);

        fixture.Logger.LogInformation("Finished AgentTests_TokenUsageStreamTest test");
    }

    [Fact]
    public async Task AgentTests_ConfigurableTest()
    {
        fixture.Logger.LogInformation("Starting AgentTests_ConfigurableTest test");

        var defaultConnector = fixture.GetAgentDefaultConnector();

        var configuration = new AgentConfiguration<ConfigurableAgentParameters>
        {
            AgentName = "ConfigurableAgent",
            Autonomous = false,
            Instructions = "You are a helpful assistant.",
            MaxOutputTokens = 100,
            Temperature = 0.7f,
            WebSearchEnabled = false,
            Parameters = new ConfigurableAgentParameters
            {
                IndexName = "ConfigurableValue"
            }
        };

        const string prompt = "What is the latest score of the Lakers game?";

        ConsoleUtilities.PrintUserMessage(prompt);

        var agent = await fixture.AgentFactory.CreateAgent<ConfigurableAgent>(defaultConnector, configuration);
        var message = fixture.AgentMessageFactory.CreateTextMessage(prompt);
        var response = await agent!.RunAsync(message);

        ConsoleUtilities.PrintAgentMessage(configuration.AgentName, response.Text);

        fixture.Logger.LogInformation("Finished AgentTests_AgentTimeTest test");
    }

    [Fact]
    public async Task AgentTests_AgentTimeTest()
    {
        fixture.Logger.LogInformation("Starting AgentTests_AgentTimeTest test");

        var defaultConnector = fixture.GetAgentDefaultConnector();

        var configuration = new AgentConfiguration
        {
            AgentName = "TimeAgent",
            Autonomous = false,
            Instructions = "You are a helpful assistant.",
            MaxOutputTokens = 100,
            Temperature = 0.7f,
            Threaded = false,
            Tools = [],
            WebSearchEnabled = false
        };

        const string prompt = "What time is the current time in UTC?";

        ConsoleUtilities.PrintUserMessage(prompt);

        var agent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, configuration);
        var message = fixture.AgentMessageFactory.CreateTextMessage(prompt);
        var response = await agent!.RunAsync(message);

        ConsoleUtilities.PrintAgentMessage(configuration.AgentName, response.Text);

        response.Should().NotBeNull();

        fixture.Logger.LogInformation("Finished AgentTests_AgentTimeTest test");
    }

    [Fact]
    public async Task AgentTests_ThreadedTest()
    {
        fixture.Logger.LogInformation("Starting AgentTests_AgentTimeTest test");

        var defaultConnector = fixture.GetAgentDefaultConnector();

        var configuration = new AgentConfiguration
        {
            AgentName = "ThreadedAgent",
            Autonomous = false,
            Instructions = "You are a helpful assistant.",
            MaxOutputTokens = 100,
            Temperature = 0.7f,
            Threaded = true,
            Tools = [],
            WebSearchEnabled = false
        };

        const string prompt = "The thread code is 12345. Remember this in the our conversation thread.";

        ConsoleUtilities.PrintUserMessage(prompt);

        var agent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, configuration)!;
        var message = fixture.AgentMessageFactory.CreateTextMessage(prompt);
        var response = await agent!.RunAsync(message);

        ConsoleUtilities.PrintAgentMessage(configuration.AgentName, response.Text);

        const string prompt2 = "What is the thread code?";

        ConsoleUtilities.PrintUserMessage(prompt2);
        var message2 = fixture.AgentMessageFactory.CreateTextMessage(prompt2);
        var response2 = await agent!.RunAsync(message2);

        ConsoleUtilities.PrintAgentMessage(configuration.AgentName, response2.Text);

        var returnCode = response2.Text.Contains("12345");

        returnCode.Should().BeTrue();

        fixture.Logger.LogInformation("Finished AgentTests_AgentTimeTest test");
    }
}