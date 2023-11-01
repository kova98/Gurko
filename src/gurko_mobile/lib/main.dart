import 'package:flutter/material.dart';
import 'package:mqtt_client/mqtt_client.dart';
import 'package:mqtt_client/mqtt_server_client.dart';
import './notification_helper.dart';

void main() => runApp(const MQTTApp());

class MQTTApp extends StatefulWidget {
  const MQTTApp({super.key});

  @override
  _MQTTAppState createState() => _MQTTAppState();
}

class _MQTTAppState extends State<MQTTApp> {
  MqttServerClient? client;
  NotificationHelper notificationHelper = NotificationHelper();

  @override
  void initState() {
    super.initState();
    connectToMQTT();
  }

  connectToMQTT() async {
    client = MqttServerClient.withPort('10.0.2.2', 'flutter_client', 1883);
    client!.logging(on: true);
    client!.onConnected = onConnected;
    client!.onDisconnected = onDisconnected;
    await client!.connect();
  }

  void onConnected() {
    print('Connected to MQTT');
    client!.subscribe('flutter/mqtt', MqttQos.atLeastOnce);
    client!.updates!.listen((List<MqttReceivedMessage<MqttMessage>> c) {
      final MqttPublishMessage message = c[0].payload as MqttPublishMessage;
      final payload =
          MqttPublishPayload.bytesToStringAsString(message.payload.message);
      print('Received message: $payload');
      notificationHelper.showNotification(
        title: 'Notification Title',
        body: 'Notification Body',
        payload: payload,
      );
    });
  }

  void onDisconnected() {
    print('Disconnected from MQTT');
  }

  @override
  Widget build(BuildContext context) {
    return MaterialApp(
      home: Scaffold(
        appBar: AppBar(title: const Text('MQTT Test App')),
        body: Center(
          child: Column(
            mainAxisAlignment: MainAxisAlignment.center,
            children: [
              ElevatedButton(
                onPressed: () {
                  final message = MqttClientPayloadBuilder();
                  message.addString('Hello from Flutter');
                  client!.publishMessage(
                      'flutter/mqtt', MqttQos.atLeastOnce, message.payload!);
                },
                child: const Text('Send Message'),
              ),
              ElevatedButton(
                onPressed: connectToMQTT,
                child: const Text('Connect'),
              ),
            ],
          ),
        ),
      ),
    );
  }
}
