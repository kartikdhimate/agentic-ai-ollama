using Microsoft.Extensions.VectorData;

namespace QdrantVectorStore;

public record ArchitectureDecision
{
    [VectorStoreKey]
    public Guid DocumentId { get; set; } = Guid.NewGuid();

    [VectorStoreData]
    public string Title { get; set; } = string.Empty;

    [VectorStoreData]
    public string Content { get; set; } = string.Empty;

    // The 1024-dimensional array representing the semantic meaning of the Content
    [VectorStoreVector(1024)]
    public ReadOnlyMemory<float> ContentVector { get; set; }
}