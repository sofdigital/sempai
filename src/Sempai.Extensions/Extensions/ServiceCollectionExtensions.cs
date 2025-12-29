// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using SofDigital.Sempai.Agents;
using SofDigital.Sempai.Agents.Factories;
using SofDigital.Sempai.Agents.Tools.Time;
using SofDigital.Sempai.Core.Agents;

namespace SofDigital.Sempai.Extensions;

/// <summary>
///     Provides extension methods for registering AI agent services and related components with an
///     <see
///         cref="IServiceCollection" />
///     in a dependency injection container.
/// </summary>
/// <remarks>
///     These extension methods simplify the setup of AI agent infrastructure by adding required services to
///     the application's dependency injection system. Use these methods during application startup to ensure all necessary
///     agent services are available for injection throughout the application.
/// </remarks>
public static class ServiceCollectionExtensions
{
    /// <param name="services">The service collection to which the GenericAgent will be added. Cannot be null.</param>
    extension(IServiceCollection services)
    {
        /// <summary>
        ///     Registers the GenericAgent service with the dependency injection container.
        /// </summary>
        /// <remarks>
        ///     This method adds GenericAgent as a transient service. Each request for GenericAgent will
        ///     result in a new instance.
        /// </remarks>
        /// <returns>The same IServiceCollection instance, enabling method chaining.</returns>
        public IServiceCollection AddAgents()
        {
            services.AddTransient<Agent>();
            services.AddTransient<ConfigurableAgent>();

            return services;
        }

        /// <summary>
        ///     Adds agent-related services to the specified service collection for dependency injection.
        /// </summary>
        /// <remarks>
        ///     Registers implementations for IAgentFactory and IAgentScriptEngine with scoped lifetimes.
        ///     Call this method during application startup to enable agent functionality.
        /// </remarks>
        /// <returns>The same service collection instance with agent services registered.</returns>
        public IServiceCollection AddAgentServices(IConfiguration configuration)
        {
            services.AddScoped<IAgentFactory, AgentFactory>();
            services.AddSingleton<IAgentMessageFactory, AgentMessageFactory>();
            services.AddScoped<IAgentWorkflowFactory, AgentWorkflowFactory>();

            services.AddScoped<HttpClient>();

            // Get the provider type as a string
            var providerString = configuration.GetValue<string>("DefaultSearchProvider:SearchProviderType");

            if (string.IsNullOrWhiteSpace(providerString)) return services;

            return services;
        }

        /// <summary>
        ///     Adds the AgentStringToolCollection to the service collection with scoped lifetime.
        /// </summary>
        /// <returns>The same IServiceCollection instance with the AgentStringToolCollection registered.</returns>
        public IServiceCollection AddAgentToolCollections()
        {
            return services;
        }

        /// <summary>
        ///     Adds various agent tools to the service collection for dependency injection.
        /// </summary>
        /// <returns>The same IServiceCollection instance with the agent tools registered.</returns>
        public IServiceCollection AddAgentTools()
        {
            services.AddTransient<AgentTimeTool>();

            return services;
        }

        /// <summary>
        ///     Configures application options by binding configuration sections to strongly-typed objects.
        /// </summary>
        /// <remarks>
        ///     This method binds the "DefaultAgentConnectors" and "DefaultSearchProvider" configuration
        ///     sections to the <see cref="AgentDefaultConnectors" /> and <see /> classes,
        ///     respectively. These options can then be injected into dependent services using the options pattern.
        /// </remarks>
        /// <param name="configuration">The <see cref="IConfiguration" /> instance containing the configuration data.</param>
        /// <returns>The updated <see cref="IServiceCollection" /> instance.</returns>
        public IServiceCollection AddOptionsConfiguration(IConfiguration configuration)
        {
            services.Configure<AgentDefaultConnectors>(configuration.GetSection("DefaultAgentConnectors"));

            return services;
        }

        /// <summary>
        ///     Adds all necessary services for Sempai functionality to the service collection.
        /// </summary>
        /// <returns>The same IServiceCollection instance with all SomaAI services registered.</returns>
        public IServiceCollection AddSempai(IConfiguration configuration)
        {
            services.AddAgents();
            services.AddAgentServices(configuration);
            services.AddAgentToolCollections();
            services.AddAgentTools();
            services.AddOptionsConfiguration(configuration);

            return services;
        }
    }
}