// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SofDigital.Sempai.Core.Agents;

namespace Sempai.Tests.Features.Fixtures;

public class AgentTestsFixture : DependencyFixture
{
    public readonly IAgentFactory AgentFactory;

    public readonly IAgentMessageFactory AgentMessageFactory;

    public readonly ILogger<AgentTestsFixture> Logger;

    public AgentTestsFixture()
    {
        var scope = ServiceProvider.CreateScope();
        var scopeServiceProvider = scope.ServiceProvider;

        Logger = scopeServiceProvider.GetRequiredService<ILogger<AgentTestsFixture>>();
        AgentFactory = scopeServiceProvider.GetRequiredService<IAgentFactory>();
        AgentMessageFactory = scopeServiceProvider.GetRequiredService<IAgentMessageFactory>();
    }

    public AgentConnector GetAgentDefaultConnector()
    {
        var connectors = ServiceProvider.GetService<IOptions<AgentDefaultConnectors>>()?.Value!;
        var defaultBasicConnector = new AgentConnector(
            connectors.Basic!.Provider,
            connectors.Basic.ApiKey,
            connectors.Basic.Model,
            connectors.Basic.ResourceUri);

        return defaultBasicConnector;
    }

    public new void Dispose()
    {
        base.Dispose();
    }
}