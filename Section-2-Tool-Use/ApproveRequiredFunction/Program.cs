
using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;
using System.Text.Json;

// 1. Create the raw AIFunction that performs the refund operation
AIFunction rawRefundFunction = AIFunctionFactory.Create(FinanceTools.IssueRefund);
AIFunction secureRefundTool = new ApprovalRequiredAIFunction(rawRefundFunction);

// 2. Initialize the agent with the secure tool in its capabilities
IChatClient chatClient = new OllamaApiClient(OllamaConfiguration.Endpoint, OllamaConfiguration.Model).WithLongTimeout();

AIAgent agent = chatClient.AsAIAgent(
    name: "LogisticsSupport",
    instructions: "You are a customer support agent. Help users track their orders concisely.",
    // We dynamically generate the AITool and pass it into the agent's capabilities
    tools: [secureRefundTool]
);

// 3. Create a session for the agent to maintain context
AgentSession session = await agent.CreateSessionAsync();

Console.WriteLine($"Agent '{agent.Name}' initialized. Ready for secure requests.\n");

string userPrompt = "I was charged twice for order ORD-99999. Please issue a refund for $45.50.";
Console.WriteLine($"User: {userPrompt}");

// 4. Execute the agent's response to the user's request
AgentResponse response = await agent.RunAsync(userPrompt, session);

// 5. Check if the agent has made a tool call that requires human approval
var approvalRequests = response.Messages
            .SelectMany(x => x.Contents)
            .OfType<ToolApprovalRequestContent>()
            .ToList();

if (approvalRequests.Count != 0)
{
    ToolApprovalRequestContent request = approvalRequests.First();

    var requestToolCall = (FunctionCallContent)request.ToolCall;
    string toolName = requestToolCall.Name;
    string toolArguments = JsonSerializer.Serialize(requestToolCall.Arguments);

    // Display the AI's intent to the human manager
    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine($"\n[SECURITY ALERT] Agent requests permission to execute '{toolName}'");
    Console.WriteLine($"Proposed Arguments: {toolArguments}");
    Console.Write("Do you approve this action? [Y/N]: ");
    Console.ResetColor();

    string? input = Console.ReadLine();
    bool isApproved = input?.Trim().ToUpper() == "Y";

    var reason = isApproved ? "Approved by human manager." : "Denied by human manager.";

    // 6. Send the human's decision back to the Agent to resume execution
    var approvalMessage = new ChatMessage(
        ChatRole.User,
        [request.CreateResponse(isApproved, reason)]
    );

    response = await agent.RunAsync(approvalMessage, session);
}

Console.WriteLine($"\nAgent: {response.Text}");
Console.ReadLine();
