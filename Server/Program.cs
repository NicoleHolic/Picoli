using Domain;
using Domain.Input;
using Server;


var hotspot = new Hotspot()
{
    Name = Constants.Hotspot.NAME,
    Password = Constants.Hotspot.PASSWORD,
};

var server = new PicoliServer();

server.MessageReceived += async message =>
{
    var sender = message.Sender;
    var receiver = message.Receiver;
    var topic = message.Topic;
    var content = message.Parameters;

    Console.WriteLine($"{sender} -> {receiver}: ({topic}) {content}");
};

var running = true;

while (running)
{
    var input = Console.ReadLine() ?? string.Empty;
    await HandleInput(input);
}

return;



async ValueTask<bool> HandleInput(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        return Notify(false, "Command can't be empty");
    
    var literals = input.Split(' ');

    try
    {
        var result = literals switch
        {
            [var command] => await HandleCommand(command, string.Empty, Parameters.Empty),
            [var group, var command] => await HandleCommand(group, command, Parameters.Empty),
            [var group, var command, .. var parameters] => await HandleCommand(group, command, new Parameters(parameters)),
            _ => Notify(false, "Command doesn't match to any command pattern")
        };
        
        return result;
    }
    catch (Exception e)
    {
        Console.WriteLine("!: " + e.Message);
        return false;
    }
}

async ValueTask<bool> HandleCommand(string group, string command, Parameters parameters)
{
    switch (group)
    {
        case "hi":
            return Notify(true, "Hello!");

        case "hs":
        case "hotspot":
            return await HandleHotspotCommand(command, parameters);

        case "se":
        case "ser":
        case "server":
            return await HandleServerCommand(command, parameters);

        case "e":
        case "exit":
        {
            running = false;
            return Notify(true, "Shutting down...");
        }

        case "c":
        case "clear":
        {
            Console.Clear();
            return true;
        }
        
        default:
            return Notify(false, $"Command from a group '{group}' doesn't exist");
    }
}

async ValueTask<bool> HandleHotspotCommand(string command, Parameters parameters)
{
    switch (command)
    {
        case "on":
        {
            return NotifyResult(
                await hotspot.StartAsync(), 
                "Hotspot is started successfully",
                "Error while starting the hotspot");
        }
        
        case "off":
        {
            return NotifyResult(
                await hotspot.StopAsync(), 
                "Hotspot is stopped successfully",
                "Error while stopping the hotspot");
        }
        
        case "rs":
        case "restart":
        {
            return NotifyResult(
                await hotspot.RestartAsync(), 
                "Hotspot is restarted successfully",
                "Error while restarting the hotspot");
        }

        case "nm":
        case "name":
        {
            hotspot.Name = parameters.At(0, Constants.Hotspot.NAME);
            return Notify(true, $"Hotspot name changed to {hotspot.Name}");
        }

        case "pwd":
        case "pass":
        case "password":
        {
            hotspot.Password = parameters.At(0, Constants.Hotspot.PASSWORD);
            return Notify(true, $"Hotspot password changed to {hotspot.Password}");
        }
        
        default:
            return Notify(false, $"Command '{command}' doesn't match to any command from Hotspot group");
    }
}

async ValueTask<bool> HandleServerCommand(string command, Parameters parameters)
{
    switch (command)
    {
        case "on":
        {
            return NotifyResult(
                await server.StartAsync(), 
                $"Server is started successfully",
                $"Error while starting the server");
        }

        case "off":
        {
            return NotifyResult(
                await server.StopAsync(), 
                $"Server is stopped successfully",
                $"Error while stopping the server");
        }
        
        default:
            return Notify(false, $"Command '{command}' doesn't match to any command from Server group");
    };
}

bool Notify(bool value, string message)
{
    Console.WriteLine(message);
    return value;
}

bool NotifyResult(bool value, string message, string error)
{
    Console.WriteLine(value ? message : error);
    return value;
}