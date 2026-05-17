using Domain;
using Domain.Messages;
using MQTTnet;
using MQTTnet.Server;
using Newtonsoft.Json;

namespace Server;

public sealed class PicoliServer
{
    public PicoliServer(int port = Constants.Mqtt.PORT)
    {
        Port = port;
        
        MqttServer = new MqttServerFactory()
            .CreateMqttServer(new MqttServerOptionsBuilder()
                .WithDefaultEndpoint()
                .WithDefaultEndpointPort(Port)
                .Build());
        
        MqttServer.InterceptingPublishAsync += OnInterceptingPublishAsync;
    }

    public int Port { get; set; }
    
    private MqttServer MqttServer { get; set; }

    public bool IsStarted => MqttServer.IsStarted;

    public event Func<Message, ValueTask>? MessageReceived;
    
    
    public async ValueTask<bool> StartAsync()
    {
        if (IsStarted)
            throw new InvalidOperationException("Server is already started");
        
        await MqttServer.StartAsync();
        return IsStarted;
    }
    
    public async ValueTask<bool> StopAsync()
    {
        if (!IsStarted)
            throw new InvalidOperationException("Server is not started");
        
        await MqttServer.StopAsync();
        return !IsStarted;
    }

    public async ValueTask<bool> RestartAsync()
    {
        await StopAsync();
        await StartAsync();
        return IsStarted;
    }
    
    
    private async Task OnInterceptingPublishAsync(InterceptingPublishEventArgs arg)
    {
        var payload = arg.ApplicationMessage.ConvertPayloadToString();
        var message = JsonConvert.DeserializeObject<Message>(payload);
        
        if (message is null)
        {
            arg.ProcessPublish = false;
            return;
        }

        if (message.Receiver.Equals(Constants.Messages.RECEIVER_SERVER))
        {
            arg.ProcessPublish = false;
            return;
        }
        
        if (MessageReceived is not null)
            await MessageReceived.Invoke(message);
    }
}