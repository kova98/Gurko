using Gurko.Api;
using Gurko.Api.Models;
using Gurko.Api.Persistence;

namespace Gurko.Test;

public class SubscriberServiceTests
{
    private readonly ISubscriberRepository _repo;
    private readonly SubscriberService _service;
    
    public SubscriberServiceTests()
    {
        _repo = Substitute.For<ISubscriberRepository>();
        _service = new SubscriberService(_repo);
    }
    
    [Fact]
    public async Task CreateSubscriber_SubscriberDoesNotExist_ShouldCreateSubscriber()
    {
        var req = new CreateSubscriberRequest("Test");
        _repo.Exists(req.Name).Returns(false);
        Subscriber createdSubscriber = null;
        await _repo.Create(Arg.Do<Subscriber>(s => createdSubscriber = s));
        
        var result = await _service.CreateSubscriber(req);

        createdSubscriber.Should().NotBeNull();
        createdSubscriber!.Name.Should().Be(req.Name);
        createdSubscriber.SubscriberId.Should().NotBeEmpty();
        result.Status.Should().Be(ResultStatus.Created);
        result.Value.Should().Be(createdSubscriber.SubscriberId);
    }
    
    [Fact]
    public async Task CreateSubscriber_SubscriberExists_ShouldNotCreateSubscriber()
    {
        var req = new CreateSubscriberRequest("Test");
        _repo.Exists(req.Name).Returns(true);
        
        var result = await _service.CreateSubscriber(req);

        result.Status.Should().Be(ResultStatus.Failure);
        result.Error.Should().Be("Subscriber already exists");
        await _repo.DidNotReceiveWithAnyArgs().Create(default);
    }
}