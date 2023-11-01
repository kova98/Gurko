import 'package:flutter_local_notifications/flutter_local_notifications.dart';

class NotificationHelper {
  late FlutterLocalNotificationsPlugin flutterLocalNotificationsPlugin;

  NotificationHelper() {
    var initializationSettingsAndroid =
        const AndroidInitializationSettings('app_icon');
    var initializationSettings =
        InitializationSettings(android: initializationSettingsAndroid);
    flutterLocalNotificationsPlugin = FlutterLocalNotificationsPlugin();
    flutterLocalNotificationsPlugin.initialize(initializationSettings,
        onDidReceiveNotificationResponse: onSelectNotification);
  }

  void onSelectNotification(NotificationResponse details) {
    print('notification selected');
  }

  Future<void> showNotification({
    int id = 0,
    String title = '',
    String body = '',
    String payload = '',
  }) async {
    var androidPlatformChannelSpecifics = const AndroidNotificationDetails(
        'channel_ID', 'channel_name',
        importance: Importance.max, priority: Priority.high, ticker: 'ticker');
    var platformChannelSpecifics =
        NotificationDetails(android: androidPlatformChannelSpecifics);
    await flutterLocalNotificationsPlugin
        .show(id, title, body, platformChannelSpecifics, payload: payload);
  }
}
