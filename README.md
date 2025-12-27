# Sempai

Sempai is a modular SDK for building intelligent, multi-vendor AI agents in C#.  
It provides tools and abstractions to create, configure, and run agents that can integrate with a variety of AI service
providers.

## 🤖 Compatible AI Vendors

Sempai supports integration with the following AI service providers via the `AgentProviderType` enumeration:

- Anthropic
- AzureAIFoundry
- AzureOpenAI
- GoogleGemini
- Ollama
- OpenAI
- XAI

## 🚀 Getting Started

### 📦 Installation

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

## 📝 Example Usage

### Agent Factory Pattern

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
    Instructions = "You are a helpful assistant.",
    MaxOutputTokens = 100,
    Temperature = 0.7f,
    Threaded = false,
    Tools = [],
};

var agent = await agentFactory.CreateAgent<Agent>(connector, configuration);
var message = agentMessageFactory.CreateTextMessage("What time is the current time in UTC?");
var response = await agent!.RunAsync(message);

```

### Agent Workflow Factory Pattern

- **Concurrent**: Agents each work on a task in parallel
- **Sequential**: Agents each work on a task in an ordered pipeline
- **Handoff**: Agents are orchestrated depending on a task
- **Group Chat**: Agents collaborate amongst themselves on a task

```csharp
import SofDigital.Sempai.Core;
import SofDigital.Sempai.Agents;
import SofDigital.Sempai.Extensions;

services.AddSempai();

var agentFactory = ServiceProvider.GetService<IAgentFactory>();
var agentMessageFactory = ServiceProvider.GetService<IAgentMessageFactory();
var agentWorkflowFactory = ServiceProvider.GetService<IAgentWorkflowFactory>();

var connector = new AgentConnector(AgentProviderType.OpenAI, "<api-key>", "gpt-5.2");
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

const string prompt = "Create a slogan for an eco-friendly new LLM model.";

var writerAgent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, writerAgentConfiguration);
var reviewerAgent = await fixture.AgentFactory.CreateAgent<Agent>(defaultConnector, reviewerAgentConfiguration);

var message = fixture.AgentMessageFactory.CreateTextMessage(prompt);
var messages = new List<ChatMessage> { message };

var agentSequentialWorkflow = fixture.AgentWorkflowFactory 
    .CreateGroupChat(3, writerAgent!, reviewerAgent!);

var result = await agentSequentialWorkflow.CreateStreamAsync(messages, true);

await agentSequentialWorkflow.ConsumeStreamAsync(msg =>
{
    Console.WriteLine(
        "Yielded \nauthor:{AuthorName} \nrole: {Role}\nmessage: {Message}",
        msg.AuthorName, msg.Role, msg.Contents);
});

```

## ⚙️ Configuration

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

## 📦 NuGet Packages

This project depends on the following NuGet packages:

- `Sempai`
- `Sempai.Core`
- `Sempai.Extensions`
