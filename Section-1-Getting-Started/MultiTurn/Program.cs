using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;

IChatClient chatClient = new OllamaApiClient(OllamaConfiguration.Endpoint, OllamaConfiguration.Model).WithLongTimeout();

AIAgent agent = chatClient.AsAIAgent(
    name: "HistoryBuff",
    instructions: "You are a helpful history teacher. You answer questions and help students make connections between events. Keep your answers brief."
);

AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine("Agent 'HistoryBuff' is online. Ask any history-related question, or type 'exit' to quit.\n");

while (true)
{
    Console.Write("User: ");
    string? userInput = Console.ReadLine();

    if (string.IsNullOrWhiteSpace(userInput) || userInput.Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    Console.WriteLine("\nAgent (streaming): ");
    await foreach (var response in agent.RunStreamingAsync(userInput, session))
    {
        Console.Write(response.Text);
    }
    Console.WriteLine();
}
