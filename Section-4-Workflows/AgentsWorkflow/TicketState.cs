namespace AgentsWorkflow;

public record TicketState(string UserQuery, string Category = "Unassigned", string FinalResolution = "");