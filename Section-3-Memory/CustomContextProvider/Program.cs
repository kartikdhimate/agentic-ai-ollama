using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;
using OpenAI.Chat;
using System.Text.Json;

IChatClient chatClient = new OllamaApiClient(OllamaConfiguration.Endpoint, OllamaConfiguration.Model).WithLongTimeout().AsIChatClient();

AIAgent agent = chatClient.AsAIAgent(new ChatClientAgentOptions()
{
    Name = "CorporateGuide",
    ChatOptions = new() { Instructions = "You are a friendly internal corporate assistant." },
    AIContextProviders = [new EmployeeProfileProvider(chatClient)]
});

AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine("--- Starting Fresh Session ---");
Console.WriteLine(await agent.RunAsync("What is the company policy on remote work?", session));
Console.WriteLine(await agent.RunAsync("My name is John and I work in the IT Department.", session));

JsonElement serializedSession = await agent.SerializeSessionAsync(session);

Console.WriteLine("\n--- Simulating a New Day (Deserializing Session) ---");

var resumedSession = await agent.DeserializeSessionAsync(serializedSession);

Console.WriteLine(await agent.RunAsync("Can you remind me of my department?", resumedSession));

var profileProvider = agent.GetService<EmployeeProfileProvider>();
var profile = profileProvider?.GetProfile(resumedSession);

Console.WriteLine("\n[SYSTEM DIAGNOSTICS] Explicitly reading memory component:");
Console.WriteLine($"Extracted Name: {profile?.EmployeeName}");
Console.WriteLine($"Extracted Department: {profile?.Department}");
