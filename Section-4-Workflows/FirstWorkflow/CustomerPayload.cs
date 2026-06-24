namespace FirstWorkflow;

public record CustomerPayload(string CompanyName, string Industry, bool IsValidated = false, string Status = "New");