namespace Server;

public sealed class Hotspot
{
    private const string Interface = "wlan0";
    private const string VirtualInterface = "uap0";
    private const string ConnectionName = "Hotspot";
    
    public required string Name { get; set; }
    public required string Password { get; set; }
    
    public bool IsRunning { get; set; } = false;
    
    
    public async ValueTask<bool> StartAsync()
    {
        if (IsRunning)
            throw new InvalidOperationException("Hotspot is already running!");
        
        await Terminal.RunAsync("iw", $"dev {Interface} interface add {VirtualInterface} type __ap");
        await Terminal.RunAsync("ip",  $"link set {VirtualInterface} up");

        await Terminal.RunAsync("nmcli", $"con add type wifi ifname {VirtualInterface} con-name {ConnectionName} autoconnect yes ssid \"{Name}\" mode ap ipv4.method shared");
        await Terminal.RunAsync("nmcli", $"con modify {ConnectionName} wifi-sec.key-mgmt wpa-psk wifi-sec.psk \"{Password}\"");

        var result = await Terminal.RunAsync("nmcli", $"con up {ConnectionName}");
        
        IsRunning = result;
        return result;
    }
    
    public async ValueTask<bool> StopAsync()
    {
        if (!IsRunning)
            throw new InvalidOperationException("Hotspot is not running!");
        
        var result = await Terminal.RunAsync("nmcli", $"con down {ConnectionName}");
        await ClearAsync();
        
        IsRunning = !result;
        return result;
    }

    public async ValueTask<bool> ClearAsync()
    {
        await Terminal.RunAsync("nmcli", $"con delete {ConnectionName}");
        await Terminal.RunAsync("iw", $"dev {VirtualInterface} del");
        return true;
    }

    public async ValueTask<bool> RestartAsync()
    {
        await StopAsync();
        return await StartAsync();
    }
}