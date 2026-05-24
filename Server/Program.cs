using System.Net;
using Domain;
using Domain.Input;
using Domain.Messages;
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

    Console.WriteLine($"[{message.PublishedAt}] {sender} -> {receiver}: ({topic}) {content}");
};

await HandleInitialArguments();

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

        case "i":
        case "info":
        {
            Console.WriteLine("Hotspot:");
            Console.WriteLine($"-> Running: {hotspot.IsRunning}");
            Console.WriteLine($"-> Name: {hotspot.Name}");
            Console.WriteLine($"-> Password: {hotspot.Password}");
            Console.WriteLine("Server:");
            Console.WriteLine($"-> Running: {server.IsStarted}");
            var ips = Dns.GetHostAddresses(Dns.GetHostName())
                .Where(a => a.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork);
            Console.WriteLine($"-> Ip: {string.Join(", ", ips)}");
            Console.WriteLine($"-> Port: {server.Port}");
            return true;
        }
        
        case "e":
        case "exit":
        {
            if (hotspot.IsRunning) await hotspot.StopAsync();
            if (server.IsStarted) await server.StopAsync();
            
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
                "Hotspot started successfully",
                "Error while starting the hotspot");
        }
        
        case "off":
        {
            return NotifyResult(
                await hotspot.StopAsync(), 
                "Hotspot stopped successfully",
                "Error while stopping the hotspot");
        }
        
        case "c":
        case "clear":
        {
            return NotifyResult(
                await hotspot.ClearAsync(), 
                "Hotspot configuration cleared successfully",
                "Error while clearing hotspot configuration");
        }
        
        case "rs":
        case "restart":
        {
            return NotifyResult(
                await hotspot.RestartAsync(), 
                "Hotspot restarted successfully",
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
                $"Server started successfully",
                $"Error while starting the server");
        }

        case "off":
        {
            return NotifyResult(
                await server.StopAsync(), 
                $"Server stopped successfully",
                $"Error while stopping the server");
        }
        
        case "p":
        case "pub":
        case "publish":
        {
            var content = parameters.At(0, "Hello from server");
            var topic = parameters.At(1, Constants.Messages.MESSAGE_TOPIC);

            var message = new Message()
            {
                Id = Guid.CreateVersion7(),
                PublishedAt = DateTime.Now,
                Topic = topic,
                Parameters = content,
                Receiver = Constants.Messages.RECEIVER_ALL,
                Sender = Constants.Messages.RECEIVER_SERVER,
            };

            return NotifyResult(
                await server.PublishAsync(message), 
                $"Message published successfully ({message.Id})",
                $"Error while publishing the message ({message.Id})");
        }
        
        case "s":
        case "se":
        case "send":
        {
            var receiver = parameters.At(0, Constants.Messages.RECEIVER_ALL);
            var content = parameters.At(1, "Hello from server");
            var topic = parameters.At(2, Constants.Messages.MESSAGE_TOPIC);

            var message = new Message()
            {
                Id = Guid.CreateVersion7(),
                PublishedAt = DateTime.Now,
                Topic = topic,
                Parameters = content,
                Receiver = receiver,
                Sender = Constants.Messages.RECEIVER_SERVER,
            };

            return NotifyResult(
                await server.PublishAsync(message), 
                $"Message send to {receiver} successfully ({message.Id})",
                $"Error while sending the message to {receiver} ({message.Id})");
        }
        
        default:
            return Notify(false, $"Command '{command}' doesn't match to any command from Server group");
    };
}

async ValueTask HandleInitialArguments()
{
    var parameters = new Parameters(args);

    if (parameters.Contains("hotspot", "hs"))
        NotifyResult(
            await hotspot.StartAsync(), 
            "Hotspot started successfully",
            "Error while starting the hotspot");
    
    if (parameters.Contains("server", "ser"))
        NotifyResult(
            await server.StartAsync(), 
            $"Server started successfully",
            $"Error while starting the server");
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