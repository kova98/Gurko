namespace Gurko.Api.Persistence;

public interface IConnectionRepository
{
    Task<long> Create(IConnection connection);
    IEnumerable<IConnection> GetSubscriberConnections(Guid subscriberId);
}

public interface IConnection
{
    Task Send(string message);
    Guid SubscriberId { get; }
}