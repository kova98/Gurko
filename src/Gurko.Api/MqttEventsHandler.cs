using MQTTnet.Server;
using Gurko.Persistence;
using MQTTnet.Protocol;

public class MqttEventsHandler(ILogger<MqttEventsHandler> logger, IServiceProvider serviceProvider, MqttServer mqttServer)
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

        using var scope = serviceProvider.CreateScope();
        var subscriberRepo = scope.ServiceProvider.GetRequiredService<ISubscriberRepository>();

        var exists = await subscriberRepo.Exists(subscriberId);
        if (!exists)
        {
            args.ReasonCode = MqttConnectReasonCode.ClientIdentifierNotValid;
            args.ReasonString = $"Subscriber '{subscriberId}' does not exist";
        }
    }

    public async Task HandleClientConnectedAsync(ClientConnectedEventArgs args)
    {
        using var scope = serviceProvider.CreateScope();
        var connectionRepo = scope.ServiceProvider.GetRequiredService<IConnectionRepository>();

        var subscriberIdString = args.UserProperties.FirstOrDefault(x => x.Name == "subscriberId");
        var connection = new MqttConnection(mqttServer, Guid.Parse(subscriberIdString!.Value));
        await connectionRepo.Create(connection);
    }
}