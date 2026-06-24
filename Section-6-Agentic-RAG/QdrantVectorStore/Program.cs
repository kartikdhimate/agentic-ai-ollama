using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;
using QdrantVectorStore;

// Create an Ollama chat client for the enterprise architecture agent
IChatClient chatClient = new OllamaApiClient(OllamaConfiguration.Endpoint, OllamaConfiguration.Model).WithLongTimeout().AsIChatClient();

// Create an Ollama embedding generator for semantic search
var embeddingGenerator = new OllamaApiClient(OllamaConfiguration.Endpoint, OllamaConfiguration.EmbeddingModel).WithLongTimeout();

// Create a Qdrant client and vector store
var qdrantProvider = new QdrantVectorStoreProvider("localhost", 6334);
var qdrantClient = qdrantProvider.QdrantClient;
var vectorStore = qdrantProvider.VectorStore;

// Get the specific collection containing our Architectural Decision Records (ADRs)
var adrCollection = await qdrantProvider.InitializeCollection();

// Seed the Qdrant collection with sample ADRs and generate embeddings for each record
await qdrantProvider.SeedData(embeddingGenerator);

TextSearchProviderOptions textSearchOptions = new()
{
    SearchTime = TextSearchProviderOptions.TextSearchBehavior.BeforeAIInvoke,
};

AIAgent architectAgent = chatClient.AsAIAgent(new ChatClientAgentOptions()
{
    Name = "EnterpriseArchitect",
    ChatOptions = new()
    {
        Instructions = "You are a senior enterprise architect. Always reference the provided ADR context to answer questions about past architectural decisions."
    },
    AIContextProviders = [new TextSearchProvider((query, cancellationToken) => qdrantProvider.VectorSearchAdapter(embeddingGenerator, query, cancellationToken), textSearchOptions)]
});

Console.WriteLine("--- Enterprise Architecture Swarm Online ---\n");

// The active researcher loop begins
string query = "Why did we choose gRPC over REST for the internal microservices communication in 2024 ?";
Console.WriteLine($"User: {query}");

// The agent will autonomously:
// 1. Invoke the VectorSearchAdapter with the user's query.
// 2. The adapter will embed the query and search Qdrant.
// 3. Qdrant will return the top semantic matches.
// 4. The agent will read the retrieved ADRs and synthesize the final answer.
await foreach (var response in architectAgent.RunStreamingAsync(query))
{
    Console.Write(response.Text);
}
