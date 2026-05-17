using Client;
using Domain;
using Domain.Input;
using Domain.Messages;


var client = new PicoliClient();

client.MessageReceived += async message =>
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
        {
            return Notify(true, "Hello!");
        }

        case "cl":
        case "cli":
        case "client":
            return await HandleClientCommand(command, parameters);

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
    
async ValueTask<bool> HandleClientCommand(string command, Parameters parameters)
{
    switch (command)
    {
        case "i":
        case "info":
        {
            Console.WriteLine($"Name: {client.Name}");
            Console.WriteLine($"Connected: {client.IsConnected}");
            return true;
        }
        
        case "on":
        case "con":
        case "connect":
        {
            var host = parameters.At(0, Constants.Mqtt.IP);
            var port = parameters.At(1, Constants.Mqtt.PORT);
            
            return NotifyResult(
                await client.ConnectAsync(host, port),
                $"Connected successfully to {host}:{port}",
                $"Error while connecting to {host}:{port}");
        }
        
        case "off":
        case "dis":
        case "disconnect":
        {
            return NotifyResult(
                await client.DisconnectAsync(),
                $"Disconnected successfully",
                $"Error while disconnecting");
        }
        
        case "sub":
        case "subscribe":
        {
            var topic = parameters.At(0, Constants.Messages.MESSAGE_TOPIC);
            
            return NotifyResult(
                await client.SubscribeAsync(topic), 
                $"Subscribed successfully to {topic}",
                $"Error while subscribing to {topic})");
        }
        
        case "un":
        case "uns":
        case "unsubscribe":
        {
            var topic = parameters.At(0, Constants.Messages.MESSAGE_TOPIC);
            
            return NotifyResult(
                await client.UnsubscribeAsync(topic), 
                $"Unsubscribed successfully from {topic}",
                $"Error while unsubscribing from {topic}");
        }
        
        case "p":
        case "pub":
        case "publish":
        {
            var content = parameters.At(0, "Hello from client");
            var topic = parameters.At(1, Constants.Messages.MESSAGE_TOPIC);

            var message = new Message()
            {
                Id = Guid.CreateVersion7(),
                PublishedAt = DateTime.Now,
                Topic = topic,
                Parameters = content,
                Receiver = Constants.Messages.RECEIVER_ALL,
                Sender = client.Name,
            };

            return NotifyResult(
                await client.PublishAsync(message), 
                $"Message published successfully ({message.Id})",
                $"Error while publishing the message ({message.Id})");
        }
        
        case "s":
        case "se":
        case "send":
        {
            var receiver = parameters.At(0, client.Name);
            var content = parameters.At(1, "Hello from client");
            var topic = parameters.At(2, Constants.Messages.MESSAGE_TOPIC);

            var message = new Message()
            {
                Id = Guid.CreateVersion7(),
                PublishedAt = DateTime.Now,
                Topic = topic,
                Parameters = content,
                Receiver = receiver,
                Sender = client.Name,
            };

            return NotifyResult(
                await client.PublishAsync(message), 
                $"Message send to {receiver} successfully ({message.Id})",
                $"Error while sending the message to {receiver} ({message.Id})");
        }
        
        default:
            return Notify(false, $"Command '{command}' doesn't match to any command from Client group");
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