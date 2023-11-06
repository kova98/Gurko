using Gurko.Api;
using MQTTnet.Server;
using Gurko.Persistence;
using MQTTnet.Protocol;

public class MqttEventHandler(
    IConnectionRepository connectionRepo, 
    ISubscriberRepository subscriberRepo,
    IMqttServer mqttServer)
{
    public async Task HandleConnectionValidationAsync(ValidatingConnectionEventArgs args)
    {
        var subIdString = args.UserProperties?.FirstOrDefault(x => x.Name == "subscriberId")?.Value;
        if (subIdString == null || !Guid.TryParse(subIdString, out var subscriberId))
        {
            args.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
            args.ReasonString = "Missing or invalid subscriberId";
            return;
        }

        var exists = await subscriberRepo.Exists(subscriberId);
        if (!exists)
        {
            args.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
            args.ReasonString = $"Subscriber '{subscriberId}' does not exist";
        }
    }

    public async Task HandleClientConnectedAsync(ClientConnectedEventArgs args)
    {
        var subscriberIdString = args.UserProperties.FirstOrDefault(x => x.Name == "subscriberId");
        var connection = new MqttConnection(mqttServer, Guid.Parse(subscriberIdString!.Value));
        await connectionRepo.Create(connection);
    }
}