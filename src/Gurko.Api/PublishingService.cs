using Gurko.Api.Models;
using Gurko.Api.Persistence;

namespace Gurko.Api;

public class PublishingService(ISubscriberRepository repo, IConnectionRepository connectionRepo)
{
    public async Task<Result> PublishNotification(PublishNotificationRequest req)
    {
        if (!await repo.Exists(req.subscriberId))
        {
            return Result.Fail($"Subscriber '{req.subscriberId}' does not exist");
        }
        
        foreach (var connection in connectionRepo.GetSubscriberConnections(req.subscriberId))
        {
            await connection.Send(req.content);
        }
        
        return Result.Ok();
    }
}