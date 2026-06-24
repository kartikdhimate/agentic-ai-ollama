using AgentsWorkflow;
using Microsoft.Agents.AI;
using Microsoft.Agents.AI.Workflows;
using Microsoft.Extensions.AI;
using Ollama;
using OllamaSharp;

IChatClient chatClient = new OllamaApiClient(OllamaConfiguration.Endpoint, OllamaConfiguration.Model).WithLongTimeout().AsIChatClient();

// Triage Agent: Responsible for analyzing incoming IT requests and categorizing them as 'Hardware' or 'Software'
// Acts as a router
AIAgent triageAgent = chatClient.AsAIAgent(
            name: "Triage",
            instructions: "Analyze the user's IT request. Categorize it strictly as either 'Hardware' or 'Software'. Output only the category word."
        );

// Hardware Support Agent: Specializes in diagnosing and providing troubleshooting steps for physical device issues
AIAgent hardwareAgent = chatClient.AsAIAgent(
    name: "HardwareSupport",
    instructions: "You are an enterprise hardware specialist. Provide concise troubleshooting steps for physical device issues."
);

// Software Support Agent: Focuses on resolving software-related problems, including application errors, OS issues, and network connectivity problems
AIAgent softwareAgent = chatClient.AsAIAgent(
    name: "SoftwareSupport",
    instructions: "You are an enterprise software specialist. Provide concise troubleshooting steps for application, OS, and network issues."
);

// Triage Node Execution Logic
Func<TicketState, TicketState> triageFunc = state =>
{
    Console.WriteLine($"[Triage] Analyzing ticket: '{state.UserQuery}'");
    AgentResponse response = triageAgent.RunAsync(state.UserQuery).GetAwaiter().GetResult();

    string category = response.Text.Trim();
    Console.WriteLine($"[Triage] Decision: Routed to {category} Department.");

    // Return a mutated copy of the state with the new category
    return state with { Category = category };
};
var triageNode = triageFunc.BindAsExecutor("TriageNode");

// Hardware Node Execution Logic
Func<TicketState, TicketState> hardwareFunc = state =>
{
    Console.WriteLine($"[Hardware Support] Generating resolution...");
    AgentResponse response = hardwareAgent.RunAsync(state.UserQuery).GetAwaiter().GetResult();
    return state with { FinalResolution = response.Text };
};
var hardwareNode = hardwareFunc.BindAsExecutor("HardwareNode");

// Software Node Execution Logic
Func<TicketState, TicketState> softwareFunc = state =>
{
    Console.WriteLine($"[Software Support] Generating resolution...");
    AgentResponse response = softwareAgent.RunAsync(state.UserQuery).GetAwaiter().GetResult();
    return state with { FinalResolution = response.Text };
};
var softwareNode = softwareFunc.BindAsExecutor("SoftwareNode");

// Building the workflow with conditional edges based on the triage decision
var workflow = new WorkflowBuilder(triageNode)
            // If Triage says Hardware, route to the Hardware Agent
            .AddEdge<TicketState>(triageNode, hardwareNode, condition: state =>
                state != null && state.Category.Contains("Hardware", StringComparison.OrdinalIgnoreCase))
            // If Triage says Software, route to the Software Agent
            .AddEdge<TicketState>(triageNode, softwareNode, condition: state =>
                state != null && state.Category.Contains("Software", StringComparison.OrdinalIgnoreCase))
            .Build();

Console.WriteLine("--- Incoming Enterprise IT Ticket ---\n");
var initialTicket = new TicketState("My laptop screen is flickering aggressively and the hinge feels loose.");

// Execute the Workflow Graph
await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, initialTicket);

TicketState? finalState = null;

// Observe the events as the payload travels between the agents
await foreach (WorkflowEvent evt in run.WatchStreamAsync())
{
    if (evt is ExecutorCompletedEvent executorComplete)
    {
        Console.WriteLine($"[System] -> Node '{executorComplete.ExecutorId}' completed.");

        // Cast Data to TicketState
        if (executorComplete.Data is TicketState ticketState)
        {
            finalState = ticketState;
            Console.WriteLine($"\tState: Category='{ticketState.Category}', Resolution='{(string.IsNullOrEmpty(ticketState.FinalResolution) ? "(pending)" : "set")}'");
        }
    }
}

Console.WriteLine($"\n--- Final Resolution ---\n{finalState?.FinalResolution}");
