namespace Gurko.Api.Persistence;

public class InMemorySubscriberRepository : ISubscriberRepository
{
    private readonly List<Subscriber> _subscribers = new();

    public InMemorySubscriberRepository()
    {
        _subscribers.Add(new Subscriber
        {
            Name = "subscriber",
            SubscriberId = new Guid("ba5c7671-8a7b-4eed-a4bd-8e33d1f724dd"),
        });
    }
    
    public Task<long> Create(Subscriber subscriber)
    {
        _subscribers.Add(subscriber);
        return Task.FromResult<long>(_subscribers.IndexOf(subscriber));
    }

    public Task<bool> Exists(Guid id)
    {
        return Task.FromResult(_subscribers.Any(s => s.SubscriberId == id));
    }

    public Task<bool> Exists(string name)
    {
        return Task.FromResult(_subscribers.Any(s => s.Name == name));
    }
}