// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using FluentAssertions;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging;
using Sempai.Tests.Features.Fixtures;
using SofDigital.Sempai.Agents;
using SofDigital.Sempai.Core.Agents;
using Xunit;

namespace Sempai.Tests.Features.Tests;

public class AgentWorkflowTests(AgentWorkflowTestsFixture fixture) : IClassFixture<AgentWorkflowTestsFixture>
{
    [Fact]
    public async Task AgentWorkflowTests_ConcurrentTest()
    {
        fixture.Logger.LogInformation("Starting AgentWorkflowTests_ConcurrentTest test");

        var defaultConnector = fixture.GetAgentDefaultConnector();

        var frenchAgentConfiguration = new AgentConfiguration
        {
            AgentName = "FrenchAgent",
            Instructions =
                """
                You are a translation assistant who only responds in French.
                Respond to any input by outputting the name of the input language 
                and then translating the input to French.
                """
        };

        var germanAgentConfiguration = new AgentConfiguration
        {
            AgentName = "GermanAgent",
            Instructions =
                """
                You are a translation assistant who only responds in German.
                Respond to any input by outputting the name of the input language 
                and then translating the input to German.
                """
        };

        const string prompt = "Make a funny joke about a hat.";

        var frenchAgent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, frenchAgentConfiguration);
        var germanAgent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, germanAgentConfiguration);

        var message = fixture.AgentMessageFactory.CreateTextMessage(prompt);
        var messages = new List<ChatMessage> { message };

        var concurrentWorkload = fixture.AgentWorkflowFactory
            .CreateConcurrent([frenchAgent!, germanAgent!]);

        var result = await concurrentWorkload.CreateStreamAsync(messages);

        result.Should().BeTrue();

        var collected = new List<ChatMessage>();

        await concurrentWorkload.ConsumeStreamAsync(msg =>
        {
            collected.Add(msg);
            fixture.Logger.LogInformation(
                "Yielded \nauthor:{AuthorName} \nrole: {Role}\nmessage: {Message}",
                msg.AuthorName, msg.Role, msg.Contents);
        });

        collected.Should().NotBeEmpty();
        collected.Should().OnlyContain(m => m != null);
        collected.Should().Contain(m => !string.IsNullOrWhiteSpace(m.Contents.ToString()));

        fixture.Logger.LogInformation("Finished AgentWorkflowTests_ConcurrentTest test");
    }

    [Fact]
    public async Task AgentWorkflowTests_GroupChatTest()
    {
        fixture.Logger.LogInformation("Starting AgentWorkflowTests_GroupChatTest test");

        var defaultConnector = fixture.GetAgentDefaultConnector();

        var writerAgentConfiguration = new AgentConfiguration
        {
            AgentName = "WriterAgent",
            Instructions =
                "You are a creative writer. Generate a catchy slogan and marketing copy. Be concise and impactful."
        };

        var reviewerAgentConfiguration = new AgentConfiguration
        {
            AgentName = "ReviewerAgent",
            Instructions =
                "You are a copy reviewer. Evaluate slogans for clarity, impact, and brand alignment."
        };

        const string prompt =
            """
                Create a slogan for an eco-friendly new LLM model.
            """;

        var writerAgent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, writerAgentConfiguration);
        var reviewerAgent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, reviewerAgentConfiguration);

        var message = fixture.AgentMessageFactory.CreateTextMessage(prompt);
        var messages = new List<ChatMessage> { message };

        var agentSequentialWorkflow = fixture.AgentWorkflowFactory
            .CreateGroupChat(3, writerAgent!, reviewerAgent!);

        var result = await agentSequentialWorkflow.CreateStreamAsync(messages, true);

        result.Should().BeTrue();

        var collected = new List<ChatMessage>();

        await agentSequentialWorkflow.ConsumeStreamAsync(msg =>
        {
            collected.Add(msg);
            fixture.Logger.LogInformation(
                "Yielded \nauthor:{AuthorName} \nrole: {Role}\nmessage: {Message}",
                msg.AuthorName, msg.Role, msg.Contents);
        });

        collected.Should().NotBeEmpty();
        collected.Should().OnlyContain(m => m != null);
        collected.Should().Contain(m => !string.IsNullOrWhiteSpace(m.Contents.ToString()));

        fixture.Logger.LogInformation("Finished AgentWorkflowTests_GroupChatTest test");
    }

    [Fact]
    public async Task AgentWorkflowTests_HandoffTest()
    {
        fixture.Logger.LogInformation("Starting AgentWorkflowTests_HandoffTest test");

        var defaultConnector = fixture.GetAgentDefaultConnector();

        var intentAgentConfiguration = new AgentConfiguration
        {
            AgentName = "IntentAgent",
            Instructions = "Determine what type of question was asked. Never answer yourself."
        };

        var mathAgentConfiguration = new AgentConfiguration
        {
            AgentName = "MathAgent",
            Instructions = "You are a maths expert and will answer any questions asked about mathematics"
        };

        var scienceAgentConfiguration = new AgentConfiguration
        {
            AgentName = "ScienceAgent",
            Instructions = "You are a sciences expert and will answer any questions asked about science"
        };

        const string prompt = "what is 1 + 1?";

        var intentAgent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, intentAgentConfiguration);
        var mathAgent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, mathAgentConfiguration);
        var scienceAgent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, scienceAgentConfiguration);

        var message = fixture.AgentMessageFactory.CreateTextMessage(prompt);
        var messages = new List<ChatMessage> { message };

        var agentHandoffWorkflow = fixture.AgentWorkflowFactory.CreateHandoff(
            intentAgent!,
            new AgentHandoffConfiguration.OneToMany(intentAgent!, [mathAgent!, scienceAgent!]),
            new AgentHandoffConfiguration.ManyToOne([mathAgent!, scienceAgent!], intentAgent!)
        );

        var result = await agentHandoffWorkflow.CreateStreamAsync(messages);

        result.Should().BeTrue();

        var collected = new List<ChatMessage>();

        await agentHandoffWorkflow.ConsumeStreamAsync(msg =>
        {
            collected.Add(msg);
            fixture.Logger.LogInformation(
                "Yielded \nauthor:{AuthorName} \nrole: {Role}\nmessage: {Message}",
                msg.AuthorName, msg.Role, msg.Contents);
        });

        collected.Should().NotBeEmpty();
        collected.Should().OnlyContain(m => m != null);
        collected.Should().Contain(m => !string.IsNullOrWhiteSpace(m.Contents.ToString()));

        fixture.Logger.LogInformation("Finished AgentWorkflowTests_HandoffTest test");
    }

    [Fact]
    public async Task AgentWorkflowTests_SequentialTest()
    {
        fixture.Logger.LogInformation("Starting AgentWorkflowTests_SequentialTest test");

        var defaultConnector = fixture.GetAgentDefaultConnector();

        var summaryAgentConfiguration = new AgentConfiguration
        {
            AgentName = "SummaryAgent",
            Instructions = "Summarize the any given text in exactly 25 words."
        };

        var translationAgentConfiguration = new AgentConfiguration
        {
            AgentName = "TranslationAgent",
            Instructions = "When given any text, translate it to German."
        };

        const string prompt =
            """
                Large Language Models (LLMs) are advanced artificial intelligence systems designed to understand, 
                generate, and manipulate human language. Trained on massive datasets, they can perform a variety of 
                tasks such as answering questions, summarizing text, translating languages, and even writing code. 
                LLMs work by predicting the next word in a sentence based on the context, which enables them to produce 
                coherent and contextually relevant responses across a wide range of topics.
            """;

        var summaryAgent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, summaryAgentConfiguration);
        var translationAgent =
            await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, translationAgentConfiguration);

        var message = fixture.AgentMessageFactory.CreateTextMessage(prompt);
        var messages = new List<ChatMessage> { message };

        summaryAgent.Should().NotBeNull();
        translationAgent.Should().NotBeNull();

        var agentSequentialWorkflow = fixture.AgentWorkflowFactory
            .CreateSequential(summaryAgent!, translationAgent!);

        var result = await agentSequentialWorkflow.CreateStreamAsync(messages, true);

        result.Should().BeTrue();

        var collected = new List<ChatMessage>();

        await agentSequentialWorkflow.ConsumeStreamAsync(msg =>
        {
            collected.Add(msg);
            fixture.Logger.LogInformation(
                "Yielded \nauthor:{AuthorName} \nrole: {Role}\nmessage: {Message}",
                msg.AuthorName, msg.Role, msg.Contents);
        });

        collected.Should().NotBeEmpty();
        collected.Should().OnlyContain(m => m != null);
        collected.Should().Contain(m => !string.IsNullOrWhiteSpace(m.Contents.ToString()));

        fixture.Logger.LogInformation("Finished AgentWorkflowTests_SequentialTest test");
    }
}