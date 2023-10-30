namespace Gurko.Api.Models;

public record CreateSubscriberRequest(string Name);
public record CreateSubscriberResponse(Guid SubscriberId);