using Gurko.Api;
using Gurko.Api.Models;
using Gurko.Persistence;
using MQTTnet.AspNetCore;

var builder = WebApplication.CreateBuilder(args);
    
builder.WebHost.ConfigureKestrel(o =>
{
    o.ListenAnyIP(1883, l => l.UseMqtt());
    o.ListenAnyIP(5000);
});

builder.Services.AddDbContext<AppDbContext>();
builder.Services.AddScoped<ISubscriberRepository, SqliteSubscriberRepository>();
builder.Services.AddSingleton<IConnectionRepository, InMemoryConnectionRepository>();
builder.Services.AddTransient<SubscriberService>();
builder.Services.AddTransient<PublishingService>();
builder.Services.AddTransient<MqttEventsHandler>();

builder.Services.AddHostedMqttServer(o =>
{
    o.WithDefaultEndpoint();
});

builder.Services.AddMqttConnectionHandler();
builder.Services.AddConnections();

var app = builder.Build();

app.UseRouting();

app.UseMqttServer(server =>
{
    var mqttEventsHandler = app.Services.GetRequiredService<MqttEventsHandler>();
    server.ValidatingConnectionAsync += mqttEventsHandler.HandleConnectionValidationAsync;
    server.ClientConnectedAsync += mqttEventsHandler.HandleClientConnectedAsync;
});

app.UseHttpsRedirection();

app.MapPost("/subscriber", async (CreateSubscriberRequest req, SubscriberService subscriberService) =>
    (await subscriberService.CreateSubscriber(req)).ToHttpResult("/subscriber"));

app.MapPost("/publish", async (PublishNotificationRequest req, PublishingService publishingService) =>
    (await publishingService.PublishNotification(req)).ToHttpResult());

app.Run();