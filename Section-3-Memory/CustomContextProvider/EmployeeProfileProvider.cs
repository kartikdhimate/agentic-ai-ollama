using Microsoft.Agents.AI;
using Microsoft.Extensions.AI;
using System.Text;

internal sealed class EmployeeProfileProvider : AIContextProvider
{
    // Securely binds our custom state to the current AgentSession
    private readonly ProviderSessionState<EmployeeProfile> _sessionState;
    // A secondary client used for background data extraction
    private readonly IChatClient _chatClient;

    public EmployeeProfileProvider(IChatClient chatClient) : base(null, null)
    {
        _sessionState = new ProviderSessionState<EmployeeProfile>(
            _ => new EmployeeProfile(),
            this.GetType().Name);
        _chatClient = chatClient;
    }

    public override IReadOnlyList<string> StateKeys => [_sessionState.StateKey];

    public EmployeeProfile GetProfile(AgentSession session) => _sessionState.GetOrInitializeState(session);

    // Phase 1: Pre-Invocation (Injecting Context)
    protected override ValueTask<AIContext> InvokingCoreAsync(InvokingContext context, CancellationToken cancellationToken = default)
    {
        var profile = _sessionState.GetOrInitializeState(context.Session);
        StringBuilder instructions = new();

        // Dynamically build instructions based on the current known state
        instructions
            .AppendLine(profile.EmployeeName is null ?
                "Ask the user for their name and politely decline to answer corporate questions until they provide it." :
                $"The user's name is {profile.EmployeeName}.")
            .AppendLine(profile.Department is null ?
                "Ask the user for their department (e.g., HR, IT, Finance) and politely decline to answer corporate questions until they provide it." :
                $"The user's department is {profile.Department}. Tailor your answers to this department.");

        // Inject the dynamically generated instructions and the conversation history into the AIContext for this turn
        AIContext result = new()
        {
            Instructions = instructions.ToString(),
            Messages = context.AIContext.Messages
        };
        return new ValueTask<AIContext>(result);
    }

    // Phase 2: Post-Invocation (Extracting & Storing State)
    protected override async ValueTask InvokedCoreAsync(InvokedContext context, CancellationToken cancellationToken = default)
    {
        var profile = _sessionState.GetOrInitializeState(context.Session);

        // If we are missing data, run a lightweight background LLM extraction on the user's messages
        if ((profile.EmployeeName is null || profile.Department is null) && context.RequestMessages.Any(x => x.Role == ChatRole.User))
        {
            var result = await _chatClient.GetResponseAsync<EmployeeProfile>(
                context.RequestMessages,
                new ChatOptions() { Instructions = "Extract the user's name and corporate department from the message if present. If not present, return nulls." },
                cancellationToken: cancellationToken);

            // Update state with extracted data
            profile.EmployeeName ??= result.Result?.EmployeeName;
            profile.Department ??= result.Result?.Department;
        }

        // Save the updated state back to the session
        _sessionState.SaveState(context.Session, profile);
    }
}