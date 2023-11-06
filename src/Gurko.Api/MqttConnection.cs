using Gurko.Persistence;
using MQTTnet;
using MQTTnet.Server;

public class MqttConnection(MqttServer mqttServer, Guid subscriberId) : IConnection
{
    public async Task Send(string message)
    {
        var mqttMessage = new MqttApplicationMessageBuilder()
            .WithTopic(SubscriberId.ToString())
            .WithPayload(message)
            .Build();

        await mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(mqttMessage));
    }

    public Guid SubscriberId { get; } = subscriberId;
}