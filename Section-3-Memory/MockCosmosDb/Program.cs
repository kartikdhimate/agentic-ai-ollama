using MockCosmosDb;

var repository = new MockCosmosDbRepository();
var agentService = new StatelessAgentService(repository);

string userId = "user-778899";

Console.WriteLine("--- Monday Morning ---");
string response1 = await agentService.HandleUserMessageAsync(userId, "Hi, I am planning a trip to Tokyo next month.");
Console.WriteLine($"Agent: {response1}\n");

// The application could completely shut down or restart here.
// The memory is safely stored in the repository.

Console.WriteLine("--- Friday Afternoon (Simulating a new server request) ---");
string response2 = await agentService.HandleUserMessageAsync(userId, "Do you remember where I said I was traveling?");
Console.WriteLine($"Agent: {response2}\n");
