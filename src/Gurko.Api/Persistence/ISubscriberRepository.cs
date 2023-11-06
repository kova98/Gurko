namespace Gurko.Api.Persistence;

public interface ISubscriberRepository
{
    public Task<long> Create(Subscriber subscriber); 
    public Task<bool> Exists(Guid id);
    public Task<bool> Exists(string name);
}