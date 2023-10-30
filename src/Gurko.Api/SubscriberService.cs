using Gurko.Api.Models;
using Gurko.Api.Persistence;

namespace Gurko.Api;

public class SubscriberService(ISubscriberRepository repo)
{
    public async Task<Result<Guid>> CreateSubscriber(CreateSubscriberRequest req)
    {
        var subscriber = new Subscriber
        {
            Name = req.Name,
            SubscriberId = Guid.NewGuid()
        };

        if (await repo.Exists(req.Name))
        {
            return Result<Guid>.Fail("Subscriber already exists");
        }
        
        await repo.Create(subscriber);
        
        return Result<Guid>.Created(subscriber.SubscriberId);
    }
}