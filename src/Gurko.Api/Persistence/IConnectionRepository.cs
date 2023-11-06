namespace Gurko.Api.Persistence;

public interface IConnectionRepository
{
    Task<long> Create(IConnection connection);
    Task<bool> Exists(string name);
    IEnumerable<IConnection> GetSubscriberConnections(string subscriberId);
}

public interface IConnection
{
    Task Send(string message);
}