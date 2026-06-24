using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;
using OpenAI.Chat;
using System.Text.Json;

namespace MockCosmosDb;

public class StatelessAgentService
{
    private readonly AIAgent _agent;
    private readonly ISessionRepository _repository;

    public StatelessAgentService(ISessionRepository repository)
    {
        _repository = repository;

        _agent = new OllamaApiClient(OllamaConfiguration.Endpoint, OllamaConfiguration.Model)
            .WithLongTimeout()
            .AsIChatClient()
            .AsAIAgent(
            new ChatClientAgentOptions()
            {
                Name = "PersistentGuide",
                ChatOptions = new() { Instructions = "You are a helpful assistant. You remember details across long periods of time." }
            });
    }

    public async Task<string> HandleUserMessageAsync(string sessionId, string userMessage)
    {
        AgentSession session;

        // Step A: Attempt to retrieve historical state from the database
        string? savedSessionJson = await _repository.GetSessionJsonAsync(sessionId);

        if (!string.IsNullOrEmpty(savedSessionJson))
        {
            // Parse the database string back into a JsonElement
            using JsonDocument doc = JsonDocument.Parse(savedSessionJson);

            // Step B: Deserialize the session, restoring the agent's memory
            session = await _agent.DeserializeSessionAsync(doc.RootElement);
            Console.WriteLine($"[SYSTEM LOG] Successfully restored session {sessionId} from database.");
        }
        else
        {
            // Step C: Fallback - Create a brand new session if no history exists
            session = await _agent.CreateSessionAsync();
            Console.WriteLine($"[SYSTEM LOG] Created new session for {sessionId}.");
        }

        // Step D: Execute the agent with the loaded session
        AgentResponse response = await _agent.RunAsync(userMessage, session);

        // Step E: Serialize the newly updated session state
        JsonElement updatedSessionElement = await _agent.SerializeSessionAsync(session);
        string updatedJsonString = JsonSerializer.Serialize(updatedSessionElement);

        // Step F: Persist the updated state back to the database
        await _repository.SaveSessionJsonAsync(sessionId, updatedJsonString);

        return response.Text;
    }
}