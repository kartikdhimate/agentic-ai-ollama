using BasicTextRAGExample;
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;
using OpenAI.Chat;

IChatClient client = new OllamaApiClient(OllamaConfiguration.Endpoint, OllamaConfiguration.Model).WithLongTimeout().AsIChatClient();

// Configure the options for the TextSearchProvider
TextSearchProviderOptions textSearchOptions = new()
{
    SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
};

// Create the AI agent with the TextSearchProvider as the AI context provider
AIAgent hrAgent = client.AsAIAgent(new ChatClientAgentOptions()
{
    Name = "HRAssistant",
    ChatOptions = new()
    {
        Instructions = "You are an HR policy assistant. Answer questions using the provided context and cite the source document when available."
    },
    AIContextProviders = [new TextSearchProvider(CustomSearchProvider.SearchAdapter, textSearchOptions)]
});

Console.WriteLine($"Agent '{hrAgent.Name}' is online with In-Memory RAG.\n");

// The agent will autonomously use the TextSearchProvider to find relevant policy information
string prompt = "I am a Tier 1 employee. What days do I need to be in the office, and how much can I expense for a new monitor?";
Console.WriteLine($"User: {prompt}");

AgentResponse response = await hrAgent.RunAsync(prompt);
Console.WriteLine($"Agent: {response.Text}");
