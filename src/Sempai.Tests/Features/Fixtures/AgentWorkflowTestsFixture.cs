// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using SofDigital.Sempai.Core.Agents;

namespace Sempai.Tests.Features.Fixtures;

public class AgentWorkflowTestsFixture : DependencyFixture
{
    public readonly IAgentFactory AgentFactory;

    public readonly IAgentMessageFactory AgentMessageFactory;

    public readonly IAgentWorkflowFactory AgentWorkflowFactory;

    public readonly ILogger<AgentWorkflowTestsFixture> Logger;

    public AgentWorkflowTestsFixture()
    {
        var scope = ServiceProvider.CreateScope();
        var scopeServiceProvider = scope.ServiceProvider;

        Logger = scopeServiceProvider.GetRequiredService<ILogger<AgentWorkflowTestsFixture>>();
        AgentFactory = scopeServiceProvider.GetRequiredService<IAgentFactory>();
        AgentMessageFactory = scopeServiceProvider.GetRequiredService<IAgentMessageFactory>();
        AgentWorkflowFactory = scopeServiceProvider.GetRequiredService<IAgentWorkflowFactory>();
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