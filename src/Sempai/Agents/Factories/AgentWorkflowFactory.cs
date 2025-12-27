// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using SofDigital.Sempai.Core;
using SofDigital.Sempai.Core.Agents;

namespace SofDigital.Sempai.Agents.Factories;

/// <inheritdoc />
public class AgentWorkflowFactory : IAgentWorkflowFactory
{
    /// <inheritdoc />
    public IAgentWorkflow CreateConcurrent(
        IEnumerable<IAgent> agents,
        Func<IList<List<ChatMessage>>, List<ChatMessage>>? aggregator = null)
    {
        var agentWorkflow = new AgentWorkflow
        {
            Agents = agents,
            WorkflowType = AgentWorkflowType.Concurrent,
            Workflow = AgentWorkflowBuilder
                .BuildConcurrent(agents.Select(x => x.ChatClientAgent).ToList(), aggregator)
        };

        return agentWorkflow;
    }

    /// <inheritdoc />
    public IAgentWorkflow CreateGroupChat(int maxIterations = 3, params IAgent[] agents)
    {
        return CreateGroupChat<AgentGroupChatManager>(maxIterations, null, agents);
    }

    /// <inheritdoc />
    public IAgentWorkflow CreateGroupChat<T>(
        int maxIterations = 3,
        Func<IReadOnlyList<IAgent>, T>? customManagerFactory = null,
        params IAgent[] agents) where T : AgentGroupChatManager
    {
        if (agents is null || agents.Length == 0)
            throw new ArgumentException("Agents list cannot be null or empty.", nameof(agents));

        var participants = agents
            .Select(a => (AIAgent)(a.ChatClientAgent
                                   ?? throw new InvalidOperationException("All agents must have a ChatClientAgent.")))
            .ToList();

        Func<IReadOnlyList<AIAgent>, AgentWorkflowBuilder.RoundRobinGroupChatManager> managerFactory =
            customManagerFactory is null
                ? aiAgents => new AgentWorkflowBuilder.RoundRobinGroupChatManager(aiAgents)
                : _ => customManagerFactory(agents);

        var agentWorkflow = new AgentWorkflow
        {
            Agents = agents,
            WorkflowType = AgentWorkflowType.GroupChat,
            Workflow = AgentWorkflowBuilder
                .CreateGroupChatBuilderWith(aiAgents =>
                {
                    var manager = managerFactory(aiAgents);
                    manager.MaximumIterationCount = maxIterations;
                    return manager;
                })
                .AddParticipants(participants)
                .Build()
        };

        return agentWorkflow;
    }

    /// <inheritdoc />
    public IAgentWorkflow CreateHandoff(IAgent initialAgent,
        params AgentHandoffConfiguration[] handoffSpecifications)
    {
        var agentWorkflow = new AgentWorkflow();
        var allAgents = new HashSet<IAgent> { initialAgent };

        foreach (var spec in handoffSpecifications)
            switch (spec)
            {
                case AgentHandoffConfiguration.OneToMany oneToMany:
                    allAgents.Add(oneToMany.From);
                    foreach (var to in oneToMany.To) allAgents.Add(to);

                    break;
                case AgentHandoffConfiguration.ManyToOne manyToOne:
                    foreach (var from in manyToOne.From) allAgents.Add(from);

                    allAgents.Add(manyToOne.To);
                    break;
                case AgentHandoffConfiguration.OneToOne oneToOne:
                    allAgents.Add(oneToOne.From);
                    allAgents.Add(oneToOne.To);
                    break;
            }

        agentWorkflow.Agents = allAgents;
        agentWorkflow.WorkflowType = AgentWorkflowType.Handoff;

        var builder = AgentWorkflowBuilder.CreateHandoffBuilderWith(initialAgent.ChatClientAgent!);

        builder = handoffSpecifications
            .Aggregate(builder, (current, spec) =>
                spec switch
                {
                    AgentHandoffConfiguration.OneToMany oneToMany =>
                        current.WithHandoffs(
                            oneToMany.From.ChatClientAgent!,
                            oneToMany.To.Select(x => x.ChatClientAgent!)),
                    AgentHandoffConfiguration.ManyToOne manyToOne =>
                        current.WithHandoffs(
                            manyToOne.From.Select(x => x.ChatClientAgent!),
                            manyToOne.To.ChatClientAgent!,
                            manyToOne.HandoffReason),
                    AgentHandoffConfiguration.OneToOne oneToOne =>
                        current.WithHandoff(
                            oneToOne.From.ChatClientAgent!,
                            oneToOne.To.ChatClientAgent!,
                            oneToOne.HandoffReason),
                    _ => current
                });

        agentWorkflow.Workflow = builder.Build();

        return agentWorkflow;
    }

    /// <inheritdoc />
    public IAgentWorkflow CreateSequential(params IAgent[] agents)
    {
        var agentWorkflow = new AgentWorkflow
        {
            Agents = agents,
            WorkflowType = AgentWorkflowType.Sequential,
            Workflow = AgentWorkflowBuilder
                .BuildSequential(agents.Select(x => x.ChatClientAgent!).ToList())
        };

        return agentWorkflow;
    }
}