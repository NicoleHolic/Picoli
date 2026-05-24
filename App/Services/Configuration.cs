using System.Text.Json;
using Client;
using Domain;

namespace App.Services;

public class Configuration
{
    public string ClientName { get; set; }
    
    public string MessageChannel { get; set; }
    
    public string DefaultIp { get; set; }
    
    public int DefaultPort { get; set; }


    public static readonly Configuration Default = new()
    {
        ClientName = PicoliClient.GenerateClientName(),
        MessageChannel = Constants.Messages.MESSAGE_TOPIC,
        DefaultIp = Constants.Mqtt.IP,
        DefaultPort = Constants.Mqtt.PORT
    };
    
    private static readonly string FilePath = FileSystem.AppDataDirectory + "/configuration.json";
    
    public static Configuration Load()
    {
        if (!File.Exists(FilePath))
            return Default.Save();
        
        try
        {
            var json = File.ReadAllText(FilePath);
            var data = JsonSerializer.Deserialize<Configuration>(json);
            
            return data ?? throw new Exception("Could not load configuration");
        }
        catch (Exception e)
        {
            return Default.Save();
        }
    }
    
    public Configuration Save()
    {
        var json =
            JsonSerializer.Serialize(this);

        File.WriteAllText(
            FilePath,
            json);

        return this;
    }
}