using Microsoft.Agents.AI;

namespace BasicTextRAGExample;

internal static class CustomSearchProvider
{
    private const string CorporatePolicy = @"
        [Effective Jan 2026] Contoso Corporate Remote Work Policy:
        - Tier 1 Employees: Must be in the office 3 days a week (Tuesday, Wednesday, Thursday).
        - Tier 2 Employees: Fully remote permitted.
        - Hardware Budget: $500 every two years for home office equipment.
        - Security: All remote work must be conducted over the corporate VPN using an issued device.
    ";

    /// <summary>
    /// Mock search adapter that searches the corporate policy based on query keywords.
    /// In production, this would connect to Azure AI Search or another search technology.
    /// </summary>
    internal static Task<IEnumerable<TextSearchProvider.TextSearchResult>> SearchAdapter(string query, CancellationToken cancellationToken)
    {
        List<TextSearchProvider.TextSearchResult> results = [];

        // Search for office/remote work related queries
        if (query.Contains("office", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("remote", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("tier", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("days", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new()
            {
                SourceName = "Contoso Corporate Remote Work Policy",
                SourceLink = "https://contoso.com/policies/remote-work",
                Text = "Tier 1 Employees: Must be in the office 3 days a week (Tuesday, Wednesday, Thursday). Tier 2 Employees: Fully remote permitted."
            });
        }

        // Search for hardware/budget related queries
        if (query.Contains("hardware", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("budget", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("expense", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("monitor", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("equipment", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new()
            {
                SourceName = "Contoso Corporate Remote Work Policy",
                SourceLink = "https://contoso.com/policies/remote-work#hardware",
                Text = "Hardware Budget: $500 every two years for home office equipment."
            });
        }

        // Search for security related queries
        if (query.Contains("security", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("vpn", StringComparison.OrdinalIgnoreCase) ||
            query.Contains("device", StringComparison.OrdinalIgnoreCase))
        {
            results.Add(new()
            {
                SourceName = "Contoso Corporate Remote Work Policy",
                SourceLink = "https://contoso.com/policies/remote-work#security",
                Text = "Security: All remote work must be conducted over the corporate VPN using an issued device."
            });
        }

        // If no specific matches, return full policy
        if (results.Count == 0)
        {
            results.Add(new()
            {
                SourceName = "Contoso Corporate Remote Work Policy",
                SourceLink = "https://contoso.com/policies/remote-work",
                Text = CorporatePolicy
            });
        }

        return Task.FromResult<IEnumerable<TextSearchProvider.TextSearchResult>>(results);
    }
}
