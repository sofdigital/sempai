# Sempai

Sempai is a modular and extensible framework for building intelligent agents. It provides tools and abstractions to
create, configure, and run agents with various capabilities.

## Getting Started

### Installation

To use Sempai in your project, you can reference the NuGet packages directly from GitHub.

```bash
dotnet nuget add source https://nuget.pkg.github.com/sofdigital/index.json -n github
```

Add the following `PackageReference` to your `.csproj` file:

```xml
<ItemGroup>
  <PackageReference Include="Sempai" Version="0.1.0" />
  <PackageReference Include="Sempai.Extensions" Version="0.1.0" />
</ItemGroup>
```

## Configuration

The application uses `appsettings.json` and environment variables for configuration. Ensure the following files are
present in the root directory:

- `appsettings.json`
- `appsettings.Development.json`

You can customize these files to set up your environment. For example:

**appsettings.json**

```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft": "Warning",
      "Microsoft.Hosting.Lifetime": "Information"
    }
  },
  "AgentDefaultConnectors": {
    "Basic": {
      "Provider": "YourProvider",
      "ApiKey": "YourApiKey",
      "Model": "YourModel",
      "ResourceUri": "YourResourceUri"
    }
  }
}
```

## Example Usage

```csharp
import SofDigital.Sempai.Core;
import SofDigital.Sempai.Agents;
import SofDigital.Sempai.Extensions;

services.AddSempai();

var agentFactory = ServiceProvider.GetService<IAgentFactory>();
var agentMessageFactory = ServiceProvider.GetService<IAgentMessageFactory();

var connector = new AgentConnector(AgentProviderType.OpenAI, "<api-key>", "gpt-5.2");
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

var agent = await agentFactory.CreateAgent<Agent>(connector, configuration);
var message = agentMessageFactory.CreateTextMessage("What time is the current time in UTC?");
var response = await agent!.RunAsync(message);

```

## NuGet Packages

This project depends on the following NuGet packages:

- `Sempai`
- `Sempai.Core`
- `Sempai.Extensions`