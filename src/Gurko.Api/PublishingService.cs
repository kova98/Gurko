using Gurko.Api.Models;
using Gurko.Api.Persistence;

namespace Gurko.Api;

public class PublishingService(ISubscriberRepository repo, IConnectionRepository connectionRepo)
{
    public async Task<Result> PublishNotification(PublishNotificationRequest req)
    {
        if (!Guid.TryParse(req.subscriberId, out var subscriberId))
        {
            return Result.Fail("Invalid subscriber id.");
        }
        
        if (!await repo.Exists(subscriberId))
        {
            return Result.Fail($"Subscriber '{subscriberId}' does not exist.");
        }
        
        foreach (var connection in connectionRepo.GetSubscriberConnections(subscriberId))
        {
            await connection.Send(req.content);
        }
        
        return Result.Ok();
    }
}