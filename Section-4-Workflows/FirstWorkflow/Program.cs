using FirstWorkflow;
using Microsoft.Agents.AI.Workflows;

// Validation Node: Validates incoming customer data and updates the payload status accordingly
Func<CustomerPayload, CustomerPayload> validateFunc = payload =>
{
    Console.WriteLine($"[Validator] Inspecting payload for: {payload.CompanyName}");
    bool isValid = !string.IsNullOrWhiteSpace(payload.CompanyName);
    return payload with { IsValidated = isValid, Status = isValid ? "Validated" : "Rejected" };
};
var validatorExecutor = validateFunc.BindAsExecutor("ValidationNode");

// Enrichment Node: Enriches the payload with industry-specific templates and updates the status
Func<CustomerPayload, CustomerPayload> enrichFunc = payload =>
{
    Console.WriteLine($"[Enricher] Applying '{payload.Industry}' enterprise templates...");
    return payload with { Status = "Enriched" };
};
var enricherExecutor = enrichFunc.BindAsExecutor("EnrichmentNode");

// Audit Node: Logs the final state of the payload to a database and updates the status to 'Completed'
Func<CustomerPayload, CustomerPayload> auditFunc = payload =>
{
    Console.WriteLine($"[Auditor] Logging final state to database. Final Status: {payload.Status}");
    return payload;
};
var auditExecutor = auditFunc.BindAsExecutor("AuditNode");

// Building the workflow with conditional edges based on validation results
var workflow = new WorkflowBuilder(validatorExecutor)
            // Conditional Edge: Only enrich if valid
            .AddEdge<CustomerPayload>(validatorExecutor, enricherExecutor, condition: p => p?.IsValidated == true)
            // Conditional Edge: If invalid, skip to audit
            .AddEdge<CustomerPayload>(validatorExecutor, auditExecutor, condition: p => p?.IsValidated == false)
            // Standard Edge: Enrichment always flows to Audit
            .AddEdge(enricherExecutor, auditExecutor)
            .Build();

Console.WriteLine("--- Starting Workflow Execution ---\n");

var initialPayload = new CustomerPayload("Contoso Pharmaceuticals", "Healthcare");

// Execute the workflow in-process and obtain a streaming run to observe real-time events
await using StreamingRun run = await InProcessExecution.RunStreamingAsync(workflow, initialPayload);

// Listen to the stream to observe the nodes completing their work
await foreach (WorkflowEvent evt in run.WatchStreamAsync())
{
    if (evt is ExecutorCompletedEvent executorComplete)
    {
        Console.WriteLine($"[System] -> Node '{executorComplete.ExecutorId}' completed successfully.\n");
    }
}

Console.WriteLine("--- Workflow Complete ---");