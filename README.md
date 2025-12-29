# Sempai

Sempai is a modular SDK for building intelligent, multi-vendor AI agents in C#.  
It provides tools and abstractions to create, configure, and run agents that can integrate with a variety of AI service
providers.

## 🤖 Compatible AI Vendors

Sempai supports integration with the following AI service providers via the `AgentProviderType` enumeration:

- Anthropic
- AzureAIFoundry
- AzureOpenAI
- AwsBedrock
- GithubModels
- GoogleGemini
- Groq
- Huggingface
- Mistral
- Ollama
- OpenAI
- OpenRouter
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

var defaultConnector = new AgentConnector(AgentProviderType.OpenAI, "<api-key>", "gpt-5.2");
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

var writerAgent = await agentFactory.CreateAgent<Agent>(defaultConnector, writerAgentConfiguration);
var reviewerAgent = await agentFactory.CreateAgent<Agent>(defaultConnector, reviewerAgentConfiguration);

var message = agentMessageFactory.CreateTextMessage(prompt);
var messages = new List<ChatMessage> { message };

var agentSequentialWorkflow = agentWorkflowFactory 
    .CreateGroupChat(3, writerAgent!, reviewerAgent!);

var result = await agentSequentialWorkflow.CreateStreamAsync(messages, true);

await agentSequentialWorkflow.ConsumeStreamAsync(msg =>
{
    Console.WriteLine(
        "Yielded \nauthor:{AuthorName} \nrole: {Role}\nmessage: {Message}",
        msg.AuthorName, msg.Role, msg.Contents);
});

```

## 🧠 Agent Class

The `Agent` class is the core component of Sempai, providing a unified interface for interacting with various AI
vendors.

### Agent Setup

```csharp
import SofDigital.Sempai.Core;
import SofDigital.Sempai.Agents;
import SofDigital.Sempai.Extensions;

services.AddSempai();

var agentFactory = ServiceProvider.GetService<IAgentFactory>();

var connector = new AgentConnector(AgentProviderType.OpenAI, "<api-key>", "gpt-5.2");
var customAgentConfiguration = new AgentConfiguration
{
    AgentName = "CustomAgent",
    Instructions =
        "You are a creative writer. Generate a catchy slogan and marketing copy. Be concise and impactful."
};

var customAgent = await agentFactory.CreateAgent<Agent>(connector, customAgentConfiguration);


```

### Token Usage Tracking

Sempai automatically tracks token usage for each agent instance. You can access the cumulative token counts via the
following properties:

- `TokenCountInput`: Total input tokens used.
- `TokenCountOutput`: Total output tokens generated.
- `TokenCountReasoning`: Total reasoning tokens used (for supported models).

```csharp
var response = await agent.RunAsync("Hello, world!");
Console.WriteLine($"Input Tokens: {agent.TokenCountInput}");
Console.WriteLine($"Output Tokens: {agent.TokenCountOutput}");
```

### Real-time Token Monitoring

You can subscribe to the `OnTokenUsageUpdated` event to receive updates as tokens are consumed during an agent run.

```csharp
agent.OnTokenUsageUpdated += (usage) => 
{
    Console.WriteLine($"Incremental Input: {usage.Input}");
    Console.WriteLine($"Cumulative Input: {usage.TotalInput}");
};
```

### Agent as a Tool

Agents can be converted into tools, allowing them to be called by other agents.

```csharp
agent.AgentAsToolName = "SpecializedAgent";
agent.AgentAsToolDescription = "An agent that handles specialized tasks.";

var agentTool = agent.GetAsAgentTool();
// Use agentTool in another agent's configuration
```

### Streaming Responses

You can stream responses from the agent for a more interactive experience.

```csharp
var stream = agent.RunStreamingAsync("Tell me a long story...");
await foreach (var update in stream)
{
    Console.Write(update.Text);
}
```

### Structured Output

Sempai supports strongly-typed responses, allowing you to easily parse AI output into C# objects.

```csharp
public record WeatherInfo(string City, int Temperature);

var response = await agent.RunAsync<WeatherInfo>("What's the weather in Seattle?");
WeatherInfo info = response.Result;
Console.WriteLine($"It's {info.Temperature}°C in {info.City}.");
```

### Advanced Configuration

The `AgentConfiguration` object allows for fine-grained control over agent behavior:

- `Autonomous`: Enables an autonomous plan-act-reflect loop when set to `true`.
- `WebSearchEnabled`: Enables the agent to perform web searches to retrieve real-time information.
- `Instructions`: The system prompt or instructions for the agent.
- `MaxOutputTokens`: The maximum number of tokens in the response.
- `Temperature`: Randomness of the output (defaults to 0.7).
- `Threaded`: Automatically manages conversation threads when set to `true`.

### Built-in Agent Tools

The default `Agent` implementation includes built-in tools that are automatically available:

- **Current Date and Time**: Provides the agent with the ability to retrieve the current date and time in either Local
  or UTC format.

### Creating AI Tools

You can extend an agent's capabilities by adding custom tools. Tools are methods within your agent class that are
exposed to the AI.

1. **Define the Tool Method**: Create a method and use the `[Description]` attribute to describe its purpose and
   parameters.
2. **Register the Tool**: Override `GetTools()` and use `agentFactory.CreateFuction` to include your method.

```csharp
[Description("Description of what the tool does")]
public string MyCustomTool([Description("Description of the parameter")] string input)
{
    // Tool logic here
    return $"Result for {input}";
}

# On derived Agent class
public override IEnumerable<AITool> GetTools()
{
    return [AIFunctionFactory.Create(MyCustomTool, "my_custom_tool")];
}


# As an Agent configuration

var myCustomTool = await agentFactory.CreateFuction(MyCustomTool, "my_custom_tool", "My custon tool");

var customAgentConfiguration = new AgentConfiguration
{
    AgentName = "MyCustomToolAgent",
    Tools = [myCustomTool],
};
```

### Configurable Agent

The `ConfigurableAgent` is a specialized agent that can be initialized with custom parameters using a strongly-typed
configuration or built-in tools.

#### Customizing Configurable Agents

You can create your own specialized agents by inheriting from `ConfigurableAgent` and `ConfigurableAgentParameters`.
This pattern allows you to define custom parameters and specialized logic for your agents.

1. **Define Custom Parameters**: Inherit from `ConfigurableAgentParameters` to add your own settings.
2. **Define the Custom Agent**: Inherit from `ConfigurableAgent` and implement `IAgentParameterConsumer<TParameters>`
   for your custom parameter type.

```csharp
public class WebSearchAgentParameters : ConfigurableAgentParameters
{
    public string SearchEngine { get; set; } = "Bing";
}

public class WebSearchAgent : ConfigurableAgent, IAgentParameterConsumer<WebSearchAgentParameters>
{
    private WebSearchAgentParameters? _webParameters;

    public void ApplyParameters(WebSearchAgentParameters parameters)
    {
        _webParameters = parameters;
        // Optionally apply base parameters
        base.ApplyParameters(parameters);
    }

    public override IEnumerable<AITool> GetTools()
    {
        // Use _webParameters?.SearchEngine in your tool logic
        return [];
    }
}
```

To use your custom agent, pass your parameter type to `AgentConfiguration<TParameters>`:

```csharp
var configuration = new AgentConfiguration<WebSearchAgentParameters>
{
    AgentName = "MyWebSearchAgent",
    Parameters = new WebSearchAgentParameters
    {
        IndexName = "web-index",
        SearchEngine = "Google"
    }
};

services.AddTransient<WebSearchAgent>();
var agent = await agentFactory.CreateAgent<WebSearchAgent>(connector, configuration);
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
