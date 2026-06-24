using Microsoft.Agents.AI;
using Microsoft.Agents.AI.AGUI;
using Microsoft.Extensions.AI;
using Microsoft.Extensions.Logging.Abstractions;

using var httpClient = new HttpClient();

var aguiClient = new AGUIChatClient(
    httpClient,
    "https://localhost:7126/agui/support",
    NullLoggerFactory.Instance);

AIAgent clientAgent = aguiClient.AsAIAgent(
    name: "SupportClient",
    description: "You are a helpful enterprise support agent.");

const string prompt = "How can I reset my corporate password?";
Console.WriteLine($"User: {prompt}");
Console.Write("Agent: ");

await foreach (var update in clientAgent.RunStreamingAsync(prompt))
{
    Console.Write(update.Text);
}