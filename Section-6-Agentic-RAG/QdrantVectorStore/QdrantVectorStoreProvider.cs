using Microsoft.Agents.AI;
using Microsoft.Extensions.VectorData;
using Microsoft.SemanticKernel.Connectors.Qdrant;
using OllamaSharp;
using Qdrant.Client;
using QdrantVectorStoreType = Microsoft.SemanticKernel.Connectors.Qdrant.QdrantVectorStore;

namespace QdrantVectorStore;

/// <summary>
/// Provides a wrapper around the Qdrant vector store, including initialization, seeding with sample data, and performing vector searches.
/// How to run Qdrant locally using Podman:
/// podman run -p 6333:6333 -p 6334:6334 -v "$(pwd)/qdrant_storage:/qdrant/storage:z" qdrant/qdrant
/// </summary>
internal class QdrantVectorStoreProvider
{
    private readonly QdrantVectorStoreType _vectorStore;
    private readonly QdrantClient _qdrantClient;
    private QdrantCollection<Guid, ArchitectureDecision>? _qdrantCollection;

    public QdrantVectorStoreProvider(string host, int port)
    {
        _qdrantClient = new QdrantClient(host, port);
        _vectorStore = new QdrantVectorStoreType(_qdrantClient, ownsClient: true);
        _qdrantCollection = null;
    }

    public QdrantClient QdrantClient => _qdrantClient;
    public QdrantVectorStoreType VectorStore => _vectorStore;

    /// <summary>
    /// Initializes the Qdrant collection for storing architecture decisions. If the collection does not exist, it will be created.
    /// </summary>
    /// <returns>The initialized Qdrant collection.</returns>
    public async Task<QdrantCollection<Guid, ArchitectureDecision>> InitializeCollection()
    {
        _qdrantCollection = _vectorStore.GetCollection<Guid, ArchitectureDecision>("enterprise_adrs");
        await _qdrantCollection.EnsureCollectionExistsAsync();

        return _qdrantCollection;
    }

    /// <summary>
    /// Seeds the Qdrant collection with sample architecture decision records, generating embeddings for each record's content using the provided embedding generator.
    /// </summary>
    /// <param name="embeddingGenerator">The embedding generator used to create vector representations of the ADR content.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    public async Task SeedData(OllamaApiClient embeddingGenerator)
    {
        var sampleAdrs = new List<ArchitectureDecision>
        {
            new()
            {
                DocumentId = Guid.Parse("11111111-1111-1111-1111-111111111111"),
                Title = "ADR-001: gRPC for Internal Microservices Communication",
                Content = "In January 2024, we decided to adopt gRPC over REST for internal microservices communication. Key reasons: 1) Binary protocol (Protocol Buffers) provides 10x better performance than JSON, 2) Strong typing with .proto contracts reduces integration bugs, 3) Native support for bidirectional streaming enables real-time data flows, 4) HTTP/2 multiplexing reduces connection overhead. Trade-offs accepted: steeper learning curve and reduced browser compatibility (acceptable for internal services)."
            },
            new()
            {
                DocumentId = Guid.Parse("22222222-2222-2222-2222-222222222222"),
                Title = "ADR-002: PostgreSQL as Primary Database",
                Content = "In March 2024, we selected PostgreSQL over MongoDB for our primary database. Reasons: ACID compliance required for financial transactions, mature tooling ecosystem, excellent JSON support for semi-structured data, and lower operational costs. We use MongoDB only for specific document-heavy workloads."
            },
            new()
            {
                DocumentId = Guid.Parse("33333333-3333-3333-3333-333333333333"),
                Title = "ADR-003: Event-Driven Architecture with Kafka",
                Content = "In February 2024, we adopted Apache Kafka for event-driven communication between bounded contexts. This decouples services, enables event sourcing, and provides replay capability for debugging. RabbitMQ was rejected due to limited horizontal scaling."
            }
         };

        foreach (var adr in sampleAdrs)
        {
            var embedding = await embeddingGenerator.EmbedAsync(adr.Content);
            List<float[]> embeddings = embedding.Embeddings;
            adr.ContentVector = embeddings[0];  // Take the first embedding from the list
            if (_qdrantCollection != null)
            {
                await _qdrantCollection.UpsertAsync(adr);
            }
        }
    }

    /// <summary>
    /// Performs a vector search in the Qdrant collection using the provided query.
    /// The query is first embedded into a vector representation, and then the top 3 semantically similar architecture decisions are retrieved from the collection.
    /// </summary>
    /// <param name="embeddingGenerator">The embedding generator used to create a vector representation of the query.</param>
    /// <param name="query">The query string to search for.</param>
    /// <param name="cancellationToken">A token to monitor for cancellation requests.</param>
    /// <returns>A task representing the asynchronous operation, containing a collection of text search results.</returns>
    public async Task<IEnumerable<TextSearchProvider.TextSearchResult>> VectorSearchAdapter(OllamaApiClient embeddingGenerator, string query, CancellationToken cancellationToken)
    {
        // Generate embedding for the user's query
        var queryEmbedding = await embeddingGenerator.EmbedAsync(query, cancellationToken: cancellationToken);
        var queryVector = queryEmbedding.Embeddings[0];

        if(_qdrantCollection == null)
        {
            throw new InvalidOperationException($"Qdrant collection is not initialized. Call {nameof(InitializeCollection)}() before performing a search.");
        }

        // Search the Qdrant vector store for semantically similar ADRs (top 3 results)
        var searchOptions = new VectorSearchOptions<ArchitectureDecision>();
        var searchResults = _qdrantCollection.SearchAsync(queryVector, 3, searchOptions, cancellationToken);

        // Convert Qdrant results to TextSearchProvider results
        var results = new List<TextSearchProvider.TextSearchResult>();
        await foreach (var result in searchResults)
        {
            results.Add(new TextSearchProvider.TextSearchResult
            {
                SourceName = $"ADR: {result.Record.Title}",
                SourceLink = $"adr://{result.Record.DocumentId}",
                Text = $"Title: {result.Record.Title}\nContent: {result.Record.Content}"
            });
        }

        return results;
    }
}
