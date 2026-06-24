using FunctionCall;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;

IChatClient chatClient = new OllamaApiClient(OllamaConfiguration.Endpoint, OllamaConfiguration.Model).WithLongTimeout();

AIAgent agent = chatClient.AsAIAgent(
    name: "LogisticsSupport",
    instructions: "You are a customer support agent. Help users track their orders concisely.",
    // We dynamically generate the AITool and pass it into the agent's capabilities
    tools: [AIFunctionFactory.Create(LogisticsTools.GetOrderStatus)]
);

Console.WriteLine($"Agent '{agent.Name}' initialized. Ready to assist.\n");

// --- Execution Pattern 1: Synchronous (Non-Streaming) ---
Console.WriteLine("--- Synchronous Execution ---");
string prompt1 = "What is the status of order ORD-12345?";
Console.WriteLine($"User: {prompt1}");

AgentResponse response = await agent.RunAsync(prompt1);
Console.WriteLine($"Agent: {response.Text}\n");

Console.ReadLine();

// --- Execution Pattern 2: Real-Time (Streaming) ---
Console.WriteLine("--- Streaming Execution ---");
string prompt2 = "I need an update on ORD-99999, please.";
Console.WriteLine($"User: {prompt2}");
Console.Write("Agent: ");

await foreach (AgentResponseUpdate update in agent.RunStreamingAsync(prompt2))
{
    Console.Write(update.Text);
}
Console.WriteLine("\n");
Console.ReadLine();
