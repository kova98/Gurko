using Gurko.Api.Persistence;
using MQTTnet;
using MQTTnet.Server;

public class MqttConnection : IConnection
{
    private readonly MqttServer _mqttServer;

    public MqttConnection(MqttServer mqttServer, Guid subscriberId, string clientId, string endpoint)
    {
        _mqttServer = mqttServer;
        SubscriberId = subscriberId;
    }

    public async Task Send(string message)
    {
        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(SubscriberId.ToString())
            .WithPayload(message)
            .Build();

        await _mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(mqttMessage));
    }

    public Guid SubscriberId { get; }
}