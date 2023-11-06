namespace Gurko.Api.Persistence;

public interface IConnectionRepository
{
    Task<long> Create(IConnection connection);
    IEnumerable<IConnection> GetSubscriberConnections(string subscriberId);
}

public interface IConnection
{
    Task Send(string message);
    string Subscriber { get; }
}