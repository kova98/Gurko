using System.Collections;
using Gurko.Api;
using Gurko.Persistence;
using MQTTnet.Adapter;
using MQTTnet.Formatter;
using MQTTnet.Packets;
using MQTTnet.Protocol;
using MQTTnet.Server;

namespace Gurko.Test;

public class MqttEventsHandlerTests
{
    private readonly IConnectionRepository _connectionRepo = Substitute.For<IConnectionRepository>();
    private readonly ISubscriberRepository _subscriberRepo = Substitute.For<ISubscriberRepository>();
    private readonly IMqttServer _mqttServer = Substitute.For<IMqttServer>();
    private readonly MqttEventHandler _handler;
    public MqttEventsHandlerTests()
    {
        _handler = new MqttEventHandler(_connectionRepo, _subscriberRepo, _mqttServer);
    }
    
    [Fact]
    public async Task HandleConnectionValidationAsync_InvalidSubscriberId_ShouldSetReasonCode()
    {
        var args = ValidatingConnectionEventArgsWith(subscriberId: "invalid guid");
        
        // Act
        await _handler.HandleConnectionValidationAsync(args);

        // Assert
        args.ReasonCode.Should().Be(MqttConnectReasonCode.ClientIdentifierNotValid);
        args.ReasonString.Should().Be("Missing or invalid subscriberId");
    }

    [Fact]
    public async Task HandleConnectionValidationAsync_SubscriberIdDoesNotExist_ShouldSetReasonCode()
    {
        var validGuid = Guid.NewGuid().ToString();
        var args = ValidatingConnectionEventArgsWith(subscriberId: validGuid);
        
        await _handler.HandleConnectionValidationAsync(args);

        args.ReasonCode.Should().Be(MqttConnectReasonCode.ClientIdentifierNotValid);
        args.ReasonString.Should().Be($"Subscriber '{validGuid}' does not exist");
    }

    [Fact]
    public async Task HandleClientConnectedAsync_SubscriberIdExists_ShouldCreateConnection()
    {
        var validGuid = Guid.NewGuid().ToString();
        var args = ClientConnectedEventArgsWith(subscriberId: validGuid);
        
        await _handler.HandleClientConnectedAsync(args);

        await _connectionRepo.Received(1).Create(Arg.Is<MqttConnection>(x=>x.SubscriberId == Guid.Parse(validGuid)));
    }

    private ClientConnectedEventArgs ClientConnectedEventArgsWith(string subscriberId)
    {
        var packet = new MqttConnectPacket { ClientId = "test-client", UserProperties = new List<MqttUserProperty>
        {
            new MqttUserProperty("subscriberId", subscriberId)
        }};
        
        return new ClientConnectedEventArgs(packet, MqttProtocolVersion.V500, "endpoint", new Hashtable());
    }


    private ValidatingConnectionEventArgs ValidatingConnectionEventArgsWith(string subscriberId)
    {
        var packet = new MqttConnectPacket { ClientId = "test-client", UserProperties = new List<MqttUserProperty>
        {
            new MqttUserProperty("subscriberId", subscriberId)
        }};
        
        return new ValidatingConnectionEventArgs(packet, Substitute.For<IMqttChannelAdapter>(), new Hashtable());
    }
}