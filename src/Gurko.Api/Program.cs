// using Gurko.Api;
// using Gurko.Api.Models;
// using Gurko.Api.Persistence;
// using MQTTnet;
// using MQTTnet.AspNetCore;
// using MQTTnet.Server;
//
// var builder = WebApplication.CreateBuilder(args);
//
// builder.Services.AddSingleton<ISubscriberRepository, InMemorySubscriberRepository>();
// builder.Services.AddTransient<SubscriberService>();
//
// builder.Services.AddHostedMqttServer(
//     optionsBuilder =>
//     {
//         optionsBuilder.WithDefaultEndpoint();
//     });
//
// builder.Services.AddMqttConnectionHandler();
// builder.Services.AddConnections();
//
// var app = builder.Build();
//
// app.UseRouting();
//
// app.UseEndpoints(
//     endpoints =>
//     {
//         endpoints.MapConnectionHandler<MqttConnectionHandler>(
//             "/mqtt",
//             httpConnectionDispatcherOptions => httpConnectionDispatcherOptions.WebSockets.SubProtocolSelector =
//                 protocolList => protocolList.FirstOrDefault() ?? string.Empty);
//     });
//
// app.UseMqttServer(server =>
// {
//     server.ValidatingConnectionAsync += async (args) =>
//     {
//         Console.WriteLine("Validating connection");
//     };
//     server.ClientConnectedAsync += async (args) =>
//     {
//         Console.WriteLine("Client connected");
//     };
// });
//
// app.UseHttpsRedirection();
//
// app.MapPost("/subscriber", async (CreateSubscriberRequest req, SubscriberService s) =>
//     (await s.CreateSubscriber(req)).ToHttpResult("/subscriber"));
//
// app.Run();
//

/*
 * This sample starts a simple MQTT server which will accept any TCP connection.
 */

using MQTTnet;
using MQTTnet.Server;

var mqttFactory = new MqttFactory();

// The port for the default endpoint is 1883.
// The default endpoint is NOT encrypted!
// Use the builder classes where possible.
var mqttServerOptions = new MqttServerOptionsBuilder().WithDefaultEndpoint().Build();

// The port can be changed using the following API (not used in this example).
// new MqttServerOptionsBuilder()
//     .WithDefaultEndpoint()
//     .WithDefaultEndpointPort(1234)
//     .Build();

using (var mqttServer = mqttFactory.CreateMqttServer(mqttServerOptions))
{
    await mqttServer.StartAsync();

    Console.WriteLine("Press Enter to exit.");
    Console.ReadLine();

    // Stop and dispose the MQTT server if it is no longer needed!
    await mqttServer.StopAsync();
}