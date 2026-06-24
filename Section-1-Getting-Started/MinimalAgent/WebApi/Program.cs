using Microsoft.Agents.AI;
using Microsoft.Agents.AI.DevUI;
using Microsoft.Agents.AI.Hosting;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

// 1. Define the variables we extracted from Microsoft Foundry
var endpoint = OllamaConfiguration.Endpoint;
var deploymentName = OllamaConfiguration.Model;

// 2. Instantiate the universal chat client with OpenTelemetry GenAI instrumentation
IChatClient chatClient = new OllamaApiClient(endpoint, deploymentName).WithLongTimeout();
chatClient = chatClient.AsBuilder()
    .UseOpenTelemetry(configure: c => c.EnableSensitiveData = true)
    .Build();

builder.Services.AddSingleton(chatClient);

// 3. Define and Register the Agents
builder.AddAIAgent(
    name: "NetworkSupportAgent",
    instructions:
        """
        You are a Tier 1 IT Support Agent.
        Your answers must be concise, professional, and limited strictly to troubleshooting network and VPN connectivity.        
        Keep responses concise — 3-5 sentences per turn. Be direct and opinionated.        
        """,
    chatClient);

// 4. Register DevUI services
builder.AddDevUI();
builder.Services.AddOpenAIResponses();
builder.Services.AddOpenAIConversations();


var app = builder.Build();

// Configure the HTTP request pipeline.
app.MapDefaultEndpoints();

// Map DevUI endpoints 
app.MapDevUI();
app.MapOpenAIResponses();
app.MapOpenAIConversations();

// Map chat endpoint to trigger the agent
app.MapPost("/api/chat", async (ChatRequest request,
    [FromKeyedServices("NetworkSupportAgent")] AIAgent networkSupportAgent) =>
{
    var response = await networkSupportAgent.RunAsync(request.Message);
    return Results.Ok(new { response = response.Text });
});

app.Run();
