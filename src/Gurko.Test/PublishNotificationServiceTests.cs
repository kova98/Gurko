using Gurko.Api;
using Gurko.Api.Models;
using Gurko.Api.Persistence;

namespace Gurko.Test;

public class PublishNotificationServiceTests
{
    private readonly ISubscriberRepository _subscriberRepo = Substitute.For<ISubscriberRepository>();
    private readonly IConnectionRepository _connectionRepo = Substitute.For<IConnectionRepository>();
    private readonly PublishingService _service;

    public PublishNotificationServiceTests()
    {
        _service = new PublishingService(_subscriberRepo, _connectionRepo);
    }

    [Fact]
    public async Task PublishNotification_SubscriberDoesNotExist_ReturnsFailure()
    {
        var notification = new PublishNotificationRequest("content", "subscriberId");
        
        var result = await _service.PublishNotification(notification);

        result.Status.Should().Be(ResultStatus.Failure);
        result.Error.Should().Be($"Subscriber 'subscriberId' does not exist");
    }
    
    [Fact]
    public async Task PublishNotification_SubscriberExists_ReturnsOk()
    {
        var notification = new PublishNotificationRequest("content", "subscriberId");
        _subscriberRepo.Exists("subscriberId").Returns(true);
        
        var result = await _service.PublishNotification(notification);

        result.Status.Should().Be(ResultStatus.Ok);
    }

    [Fact]
    public async Task PublishNotification_SendsMessageToAllConnections()
    {
        var notification = new PublishNotificationRequest("content", "subscriberId");
        _subscriberRepo.Exists("subscriberId").Returns(true);
        var connections = Enumerable.Range(0, 5).Select(_ => Substitute.For<IConnection>()).ToList();
        _connectionRepo
            .GetSubscriberConnections(notification.subscriberId)
            .Returns(connections);

        await _service.PublishNotification(notification);

        foreach (var connection in connections)
        {
            await connection.Received(1).Send(notification.content);
        }
    }
}