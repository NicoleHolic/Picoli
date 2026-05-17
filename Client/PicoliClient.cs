using Domain;
using Domain.Contracts;
using Domain.Messages;
using MQTTnet;
using Newtonsoft.Json;

namespace Client;

public sealed class PicoliClient : IMessagePublisher
{
    public PicoliClient()
    {
        Name = GenerateClientName();
        MqttClient = new MqttClientFactory().CreateMqttClient();

        MqttClient.ApplicationMessageReceivedAsync += OnApplicationMessageReceivedAsync;
        MqttClient.ConnectedAsync += MqttClientOnConnectedAsync;
        MqttClient.DisconnectedAsync += MqttClientOnDisconnectedAsync;
    }
    
    public string Name { get; set; }
    
    private IMqttClient MqttClient { get; set; }

    public bool IsConnected => MqttClient.IsConnected; 
    
    public event Func<Message, ValueTask>? MessageReceived;
    public event Func<ValueTask>? Connected;
    public event Func<ValueTask>? Disconnected;

    
    public async ValueTask<bool> ConnectAsync(string host = Constants.Mqtt.IP, int port = Constants.Mqtt.PORT)
    {
        ThrowIfConnected();
        
        var result = await MqttClient.ConnectAsync(new MqttClientOptionsBuilder()
            .WithTcpServer(host, port)
            .Build());
        
        return result.ResultCode is MqttClientConnectResultCode.Success;
    }
    
    public async ValueTask<bool> DisconnectAsync()
    {
        ThrowIfDisconnected();
        await MqttClient.DisconnectAsync();
        return !IsConnected;
    }

    public async ValueTask<bool> SubscribeAsync(string topic)
    {
        ThrowIfDisconnected();
        var result = await MqttClient.SubscribeAsync(topic);
        return result.Items.Any(x => x.ResultCode is MqttClientSubscribeResultCode.GrantedQoS0);
    }
    
    public async ValueTask<bool> UnsubscribeAsync(string topic)
    {
        ThrowIfDisconnected();
        var result = await MqttClient.UnsubscribeAsync(topic);
        return result.Items.Any(x => x.ResultCode is MqttClientUnsubscribeResultCode.Success);
    }

    public async ValueTask<bool> PublishAsync(Message message)
    {
        ThrowIfDisconnected();
        
        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(message.Topic)
            .WithPayload(JsonConvert.SerializeObject(message))
            .Build();

        var result = await MqttClient.PublishAsync(mqttMessage);
        return result.IsSuccess;
    }


    private async Task OnApplicationMessageReceivedAsync(MqttApplicationMessageReceivedEventArgs args)
    {
        var payload = args.ApplicationMessage.ConvertPayloadToString();
        var message = JsonConvert.DeserializeObject<Message>(payload);
        
        if (message is null) return;
        
        if (message.Sender.Equals(Name)) return;

        if (message.Receiver.Equals(Constants.Messages.RECEIVER_ALL) || message.Receiver.Equals(Name))
            if (MessageReceived is not null)
                await MessageReceived.Invoke(message);
    }
    
    private async Task MqttClientOnConnectedAsync(MqttClientConnectedEventArgs args)
    {
        if (Connected is not null) 
            await Connected.Invoke();
    }
    
    private async Task MqttClientOnDisconnectedAsync(MqttClientDisconnectedEventArgs args)
    {
        if (Disconnected is not null) 
            await Disconnected.Invoke();
    }
    
    private void ThrowIfConnected()
    {
        if (IsConnected)
            throw new InvalidOperationException("Client is already connected");
    }
    
    private void ThrowIfDisconnected()
    {
        if (!IsConnected)
            throw new InvalidOperationException("Client is not connected");
    }

    private string GenerateClientName()
    {
        return $"Picoli_{Random.Shared.Next(100, 1000)}";
    }
}