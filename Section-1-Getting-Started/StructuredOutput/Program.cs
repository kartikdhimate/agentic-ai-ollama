using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;

IChatClient chatClient = new OllamaApiClient(OllamaConfiguration.Endpoint, OllamaConfiguration.Model).WithLongTimeout();

AIAgent agent = chatClient.AsAIAgent(
    name: "MeetingAnalyst",
    instructions: "You are an AI analyst. Extract the topic, action items, and overall sentiment from the provided transcript."
);

string transcript = "We discussed the Q4 marketing push. Sarah needs to finalize the budget by Tuesday. John will contact the ad agency. Overall, everyone felt very optimistic about the campaign.";
Console.WriteLine($"Analyzing Transcript:\n{transcript}\n");

AgentResponse<MeetingAnalysis> response = await agent.RunAsync<MeetingAnalysis>(transcript);

MeetingAnalysis? analysis = response.Result;

// 5. Utilize deterministic C# objects
if (analysis != null)
{
    Console.WriteLine($"Full Analysis: {analysis}\n");
    Console.WriteLine($"Topic: {analysis.Topic}");
    Console.WriteLine($"Sentiment: {analysis.Sentiment}");
    Console.WriteLine($"Action Items Count: {analysis.ActionItems.Length}");
    Console.WriteLine($"Action Items:");
    foreach (var item in analysis.ActionItems)
    {
        Console.WriteLine($"- {item}");
    }
}
else
{
    Console.WriteLine("Failed to analyze the transcript.");
}

Console.ReadLine();
