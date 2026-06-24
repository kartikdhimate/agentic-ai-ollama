namespace MockCosmosDb;

public class MockCosmosDbRepository : ISessionRepository
{
    private readonly Dictionary<string, string> _datastore = [];

    public Task<string?> GetSessionJsonAsync(string sessionId) =>
        Task.FromResult(_datastore.TryGetValue(sessionId, out var json) ? json : null);

    public Task SaveSessionJsonAsync(string sessionId, string jsonPayload)
    {
        _datastore[sessionId] = jsonPayload;
        return Task.CompletedTask;
    }
}