using Gurko.Api;
using Gurko.Api.Models;
using Gurko.Api.Persistence;
using MQTTnet;
using MQTTnet.AspNetCore;
using MQTTnet.Server;

var builder = WebApplication.CreateBuilder(args);
    
builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(1883, l => l.UseMqtt());
    o.ListenAnyIP(5000);
});

builder.Services.AddSingleton<ISubscriberRepository, InMemorySubscriberRepository>();
builder.Services.AddSingleton<IConnectionRepository, InMemoryConnectionRepository>();
builder.Services.AddTransient<SubscriberService>();

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
    server.ValidatingConnectionAsync += async (args) =>
    {
        Console.WriteLine("Validating connection");
    };
    server.ClientConnectedAsync += async (args) =>
    {
        Console.WriteLine("Client connected");
    };
});

app.UseHttpsRedirection();

app.MapPost("/subscriber", async (CreateSubscriberRequest req, SubscriberService s) =>
    (await s.CreateSubscriber(req)).ToHttpResult("/subscriber"));

app.MapPost("/publish", async (PublishNotificationRequest req, PublishingService s) =>
    (await s.PublishNotification(req)).ToHttpResult());

app.MapPost("testmqtt", async (MqttServer mqttServer) =>
{
    var message = new MqttApplicationMessageBuilder()
        .WithTopic("flutter/mqtt")
        .WithPayload("Test")
        .Build();

    await mqttServer.InjectApplicationMessage(new InjectedMqttApplicationMessage(message)
    {
        SenderClientId = "SenderClientId"
    });
});

app.Run();