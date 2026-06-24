using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

// Define the variables we extracted from Microsoft Foundry
var endpoint = OllamaConfiguration.Endpoint;
var deploymentName = OllamaConfiguration.Model;

// Instantiate the universal chat client with OpenTelemetry GenAI instrumentation
IChatClient chatClient = new OllamaApiClient(endpoint, deploymentName)
    .WithLongTimeout()
    .AsIChatClient()
    .AsBuilder()
    .UseOpenTelemetry(configure: c => c.EnableSensitiveData = true)
    .Build();
builder.Services.AddSingleton(chatClient);

var complianceAgent = builder.AddAIAgent(
    name: "compliance",
    instructions: "You are a strict enterprise compliance auditor. Review the provided text for GDPR violations. Be concise and authoritative."
);

// Register A2A server for the agent (required by latest A2A endpoint mapping APIs)
complianceAgent.AddA2AServer();

var app = builder.Build();

// Expose the agent via the A2A HTTP+JSON protocol
app.MapA2AHttpJson(complianceAgent, path: "/a2a/compliance");

app.Run();
