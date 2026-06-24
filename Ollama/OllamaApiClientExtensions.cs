using Microsoft.Extensions.AI;
using OllamaSharp;
using System.Reflection;

namespace Ollama;

public static class OllamaApiClientExtensions
{
    public static OllamaApiClient SetTimeout(this OllamaApiClient client, TimeSpan timeout)
    {
        ArgumentNullException.ThrowIfNull(client);

        if (timeout <= TimeSpan.Zero)
        {
            throw new ArgumentOutOfRangeException(nameof(timeout), "Timeout must be greater than zero.");
        }

        // Use reflection to access the private _client field in OllamaApiClient
        // This approach is necessary until OllamaSharp provides a public API for timeout configuration
        var httpClientField = typeof(OllamaApiClient)
            .GetField("_client", BindingFlags.NonPublic | BindingFlags.Instance);

        if (httpClientField?.GetValue(client) is HttpClient httpClient)
        {
            httpClient.Timeout = timeout;
        }
        else
        {
            throw new InvalidOperationException(
                "Unable to access the internal HttpClient of OllamaApiClient. " +
                "The OllamaSharp library structure may have changed.");
        }

        return client;
    }

    /// <summary>
    /// Sets a timeout suitable for quick queries (2 minutes).
    /// </summary>
    public static OllamaApiClient WithQuickTimeout(this OllamaApiClient client)
        => client.SetTimeout(TimeSpan.FromMinutes(2));

    /// <summary>
    /// Sets a timeout suitable for standard prompts (5 minutes).
    /// </summary>
    public static OllamaApiClient WithStandardTimeout(this OllamaApiClient client)
        => client.SetTimeout(TimeSpan.FromMinutes(5));

    /// <summary>
    /// Sets a timeout suitable for long-form generation (10 minutes).
    /// </summary>
    public static OllamaApiClient WithLongTimeout(this OllamaApiClient client)
        => client.SetTimeout(TimeSpan.FromMinutes(10));

    /// <summary>
    /// Sets a timeout suitable for very large models or slow hardware (30 minutes).
    /// </summary>
    public static OllamaApiClient WithExtendedTimeout(this OllamaApiClient client)
        => client.SetTimeout(TimeSpan.FromMinutes(30));

    /// <summary>
    /// Converts the OllamaApiClient to an IChatClient for use with Microsoft.Agents.AI.
    /// </summary>
    /// <param name="client">The OllamaApiClient instance to convert.</param>
    /// <returns>An IChatClient instance.</returns>
    public static IChatClient AsIChatClient(this OllamaApiClient client)
    {
        ArgumentNullException.ThrowIfNull(client);
        return client;
    }
}
