using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Hosting.AGUI.AspNetCore;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddHttpClient().AddLogging();
builder.Services.AddAGUI();

AIAgent enterpriseAgent = new OllamaApiClient(OllamaConfiguration.Endpoint, OllamaConfiguration.Model)
    .WithLongTimeout()
    .AsAIAgent(
        name: "EnterpriseSupportAgent",
        instructions: "You are a helpful enterprise support agent."
    );

var app = builder.Build();

// This single extension method automatically wires up HTTP POST processing and SSE streaming
app.MapAGUI("/agui/support", enterpriseAgent);

app.Run();
