namespace Gurko.Api.Persistence;

public class InMemorySubscriberRepository : ISubscriberRepository
{
    private readonly List<Subscriber> _subscribers = new();
    
    public Task<long> Create(Subscriber subscriber)
    {
        _subscribers.Add(subscriber);
        return Task.FromResult<long>(_subscribers.IndexOf(subscriber));
    }

    public Task<bool> Exists(string name)
    {
        return Task.FromResult(_subscribers.Any(s => s.Name == name));
    }
}