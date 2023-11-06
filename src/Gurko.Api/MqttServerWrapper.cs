using MQTTnet.Server;

namespace Gurko.Api;

// A wrapper interface and implementation to allow unit testing.

public interface IMqttServer
{
    Task InjectApplicationMessage(InjectedMqttApplicationMessage injectedMqttApplicationMessage);
}

public class MqttServerWrapper(MqttServer mqttServer) : IMqttServer
{
    public Task InjectApplicationMessage(InjectedMqttApplicationMessage injectedMqttApplicationMessage)
    {
        return mqttServer.InjectApplicationMessage(injectedMqttApplicationMessage);
    }
}