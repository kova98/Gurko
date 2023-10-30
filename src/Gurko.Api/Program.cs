using Gurko.Api;
using Gurko.Api.Models;
using Gurko.Api.Persistence;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<ISubscriberRepository, InMemorySubscriberRepository>();
builder.Services.AddTransient<SubscriberService>();

var app = builder.Build();

app.UseHttpsRedirection();

app.MapPost("/subscriber", async (CreateSubscriberRequest req, SubscriberService s) =>
    (await s.CreateSubscriber(req)).ToHttpResult("/subscriber"));
    
app.Run();