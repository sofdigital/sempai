// MIT License
// Sof Digital Corporation 2025
// Written By Michael Rinderle <michael@sofdigital.net>

using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Sempai.Tests.Features.Fixtures;
using Xunit;

namespace Sempai.Tests.Features.Tests;

public class DependencyTests(DependencyFixture fixture) : IClassFixture<DependencyFixture>
{
    [Fact]
    public void DependencyTests_ConfigurationServiceLoadedTest()
    {
        var config = fixture.ServiceProvider.GetService<IConfiguration>();

        config.Should().NotBeNull();
    }

    [Fact]
    public void DependencyTests_ConfigurationDefaultConnectorsLoadedTest()
    {
        if (fixture.Configuration != null)
        {
            var environment = fixture.Configuration!["DefaultAgentConnectors:Basic:Provider"];

            environment.Should().Be("AzureOpenAI");
        }
    }

    [Fact]
    public void DependencyTests_ResolveRegisteredServicesTest()
    {
        var config = fixture.ServiceProvider.GetRequiredService<IConfiguration>();

        config.Should().NotBeNull();
        config.Should().BeSameAs(fixture.Configuration);
    }
}