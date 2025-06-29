using Microsoft.AspNetCore.SignalR;

namespace ASL.CodeEngineering;

public class RoadmapHub : Hub
{
    public async Task UpdateFile(string name, string content)
    {
        await Clients.Others.SendAsync("UpdateFile", name, content);
    }
}
