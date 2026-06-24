using System.ComponentModel;

namespace WebApi;

// 1. Define the Enterprise Tool
public static class NetworkTools
{
    [Description("Checks the current status of the corporate VPN for a specific user.")]
    public static string CheckVpnStatus(
        [Description("The username of the employee, e.g., jsmith")] string username)
    {
        // Simulating a deterministic API call to a firewall or directory service
        return $"User {username} is currently DISCONNECTED. Error: IPsec tunnel timeout.";
    }
}