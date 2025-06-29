using ASL.CodeEngineering.AI;
using ASL.CodeEngineering.Api;

string projectRoot = args.Length > 0 ? args[0] : Directory.GetCurrentDirectory();
int port = args.Length > 1 && int.TryParse(args[1], out var p) ? p : 5000;

await using var server = new ApiServer(port, projectRoot, new DotnetBuildTestRunner());
await server.StartAsync();
Console.WriteLine($"API server running on http://localhost:{port}\nPress Enter to exit...");
Console.ReadLine();
