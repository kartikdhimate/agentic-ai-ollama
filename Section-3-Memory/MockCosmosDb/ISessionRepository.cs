namespace MockCosmosDb;

public interface ISessionRepository
{
    Task<string?> GetSessionJsonAsync(string sessionId);

    Task SaveSessionJsonAsync(string sessionId, string jsonPayload);
}
