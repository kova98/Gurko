using System.Collections.Concurrent;

namespace Gurko.Api.Persistence;

public class InMemoryConnectionRepository : IConnectionRepository
{
    private long _id = 0;
    private readonly ConcurrentDictionary<long, IConnection> _connections = new();
    
    public Task<long> Create(IConnection connection)
    {
        _connections.TryAdd(NextId(), connection);
        return Task.FromResult(_id);
    }
    
    public IEnumerable<IConnection> GetSubscriberConnections(string subscriberId) => _connections.Values
        .Where(x => x.Subscriber == subscriberId);
    
    private long NextId() => Interlocked.Increment(ref _id);
}