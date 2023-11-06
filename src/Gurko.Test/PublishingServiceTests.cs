using Gurko.Api;
using Gurko.Api.Models;
using Gurko.Persistence;

namespace Gurko.Test;

public class PublishingServiceTests
{
    private readonly ISubscriberRepository _subscriberRepo = Substitute.For<ISubscriberRepository>();
    private readonly IConnectionRepository _connectionRepo = Substitute.For<IConnectionRepository>();
    private readonly PublishingService _service;

    public PublishingServiceTests()
    {
        _service = new PublishingService(_subscriberRepo, _connectionRepo);
    }

    [Fact]
    public async Task PublishNotification_SubscriberDoesNotExist_ReturnsFailure()
    {
        var notification = new PublishNotificationRequest("content", Guid.NewGuid().ToString());
        
        var result = await _service.PublishNotification(notification);

        result.Status.Should().Be(ResultStatus.Failure);
        result.Error.Should().Be($"Subscriber '{notification.subscriberId}' does not exist.");
    }
    
    [Fact]
    public async Task PublishNotification_SubscriberExists_ReturnsOk()
    {
        var subId = Guid.NewGuid();
        var notification = new PublishNotificationRequest("content", subId.ToString());
        _subscriberRepo.Exists(subId).Returns(true);
        
        var result = await _service.PublishNotification(notification);

        result.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public async Task PublishNotification_InvalidSubscriberId_ReturnsFailure()
    {
        var notification = new PublishNotificationRequest("content", "invalid guid");
        
        var result = await _service.PublishNotification(notification);

        result.Status.Should().Be(ResultStatus.Failure);
        result.Error.Should().Be($"Invalid subscriber id.");
    }
    
    [Fact]
    public async Task PublishNotification_SendsMessageToAllConnections()
    {
        var subId = Guid.NewGuid();
        var notification = new PublishNotificationRequest("content", subId.ToString());
        _subscriberRepo.Exists(subId).Returns(true);
        var connections = Enumerable.Range(0, 5).Select(_ => Substitute.For<IConnection>()).ToList();
        _connectionRepo
            .GetSubscriberConnections(subId)
            .Returns(connections);

        await _service.PublishNotification(notification);

        foreach (var connection in connections)
        {
            await connection.Received(1).Send(notification.content);
        }
    }
}