using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;

var client = new OllamaApiClient(OllamaConfiguration.Endpoint, OllamaConfiguration.Model).WithLongTimeout();
AIAgent agent = client.AsAIAgent();

var response = await agent.RunAsync("Why is the sky blue?");

Console.WriteLine(response);
Console.ReadLine();
