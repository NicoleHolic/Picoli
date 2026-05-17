namespace Server;

public sealed class Hotspot
{
    public required string Name { get; set; }
    public required string Password { get; set; }
    
    public bool IsRunning { get; set; } = false;
    
    
    public async ValueTask<bool> StartAsync()
    {
        if (IsRunning)
            throw new InvalidOperationException("Hotspot is already running!");
        
        var result = await Command.RunAsync($"nmcli dev wifi hotspot ifname wlan0 ssid {Name} password {Password}");
        IsRunning = result;
        return result;
    }
    
    public async ValueTask<bool> StopAsync()
    {
        if (!IsRunning)
            throw new InvalidOperationException("Hotspot is not running!");
        
        var result = await Command.RunAsync($"nmcli connection down hotspot");
        IsRunning = !result;
        return result;
    }
    
    public async ValueTask<bool> RestartAsync()
    {
        await StopAsync();
        return await StartAsync();
    }
}