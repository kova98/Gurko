using Gurko.Api;
using Gurko.Api.Models;
using Gurko.Api.Persistence;
using MQTTnet.AspNetCore;
using MQTTnet.Protocol;

var builder = WebApplication.CreateBuilder(args);
    
builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(1883, l => l.UseMqtt());
    o.ListenAnyIP(5000);
});

builder.Services.AddSingleton<ISubscriberRepository, InMemorySubscriberRepository>();
builder.Services.AddSingleton<IConnectionRepository, InMemoryConnectionRepository>();
builder.Services.AddTransient<SubscriberService>();
builder.Services.AddTransient<PublishingService>();

builder.Services.AddHostedMqttServer(o =>
{
    o.WithDefaultEndpoint();
});

builder.Services.AddMqttConnectionHandler();
builder.Services.AddConnections();

var app = builder.Build();

app.UseRouting();

app.UseEndpoints(
    endpoints =>
    {
        endpoints.MapConnectionHandler<MqttConnectionHandler>(
            "/mqtt",
            httpConnectionDispatcherOptions => httpConnectionDispatcherOptions.WebSockets.SubProtocolSelector =
                protocolList => protocolList.FirstOrDefault() ?? string.Empty);
    });

app.UseMqttServer(server =>
{
    var connectionRepo = app.Services.GetRequiredService<IConnectionRepository>();
    var subscriberRepo = app.Services.GetRequiredService<ISubscriberRepository>();
    server.ValidatingConnectionAsync += async (args) =>
    {
        var subIdString = args.UserProperties?.FirstOrDefault(x => x.Name == "subscriberId");
        if (subIdString?.Value == null || !Guid.TryParse(subIdString?.Value, out var subscriberId))
        {
            args.ReasonCode = MqttConnectReasonCode.ProtocolError;
            args.ReasonString = "Missing subscriberId";
            return;
        }
        
        if (!await subscriberRepo.Exists(subscriberId))
        {
            args.ReasonCode = MqttConnectReasonCode.ProtocolError;
            args.ReasonString = $"Subscriber '{subscriberId}' does not exist";
        }
    };
    server.ClientConnectedAsync += async (args) =>
    {
        var subscriberId = args.UserProperties.First(x => x.Name == "subscriberId").Value;
        var connection = new MqttConnection(server, Guid.Parse(subscriberId), args.ClientId, args.Endpoint);
        await connectionRepo.Create(connection);
    };
});

app.UseHttpsRedirection();

app.MapPost("/subscriber", async (CreateSubscriberRequest req, SubscriberService subscriberService) =>
    (await subscriberService.CreateSubscriber(req)).ToHttpResult("/subscriber"));

app.MapPost("/publish", async (PublishNotificationRequest req, PublishingService publishingService) =>
    (await publishingService.PublishNotification(req)).ToHttpResult());

app.Run();