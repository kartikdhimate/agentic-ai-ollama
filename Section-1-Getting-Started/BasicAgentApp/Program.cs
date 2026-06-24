using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;

IChatClient chatClient = new OllamaApiClient(OllamaConfiguration.Endpoint, OllamaConfiguration.Model).WithLongTimeout();

AIAgent supportAgent = chatClient.AsAIAgent(
    name: "NetworkSupport",
    instructions: "You are a Tier 1 IT Support Agent. Your answers must be concise, professional, and limited strictly to troubleshooting network and VPN connectivity. Keep your answers brief."
);

Console.WriteLine($"Agent '{supportAgent.Name}' is online.\n");

string userIssue = "I am getting a DNS resolution error when connecting to the corporate VPN from a coffee shop. Keep your answers brief.";
Console.WriteLine($"User: {userIssue}\n");

// Non-streaming response
//AgentResponse response = await supportAgent.RunAsync(userIssue);
//Console.WriteLine($"Agent: {response.Text}");

// Streaming response
Console.WriteLine("Agent (streaming): ");
await foreach (var response in supportAgent.RunStreamingAsync(userIssue))
{
    Console.Write(response.Text);
}
Console.WriteLine();

Console.ReadLine();
