# Goal

A push notification server that allows third party applications to easily send notifications to their devices (eg. Firebase Cloud Messaging).
It should support mobile at first, but possibly other devices in the future.
The solution should focus on low latency and should easily handle concurrent, high-throughput traffic.
Monolith at first, with possibility of horizontal scaling later on.

# Domain

## Client

A client responsible of registering Subscribers, Devices, establishing Device Connections and publishing Notifications.

## Notification

A message sent by the Publisher to be delievered to eligible Subscribers.

## Subscriber

A user that is subscribed to receive notifications to its Devices.

Can have one or more Devices.

Doesn't represent a physical device by itself, but an aggregate that contains all user's devices, as well as general user info.

## Device

A single physical device able to receive notifications.

## Connection

A physical active connection to a device. eg. in case of a mobile device, it represents a live TCP connection.

It can send a message to its device.

# MVP Features

The following is a draft of the necessary features that an MVP should implement.

## Backend API

- Administration of Subscribers
- Accept Connections (TCP connection)
- Publishing endpoint

## Mobile client

- Establish Connection on startup
- Receive Notifications through the Connection

# MVP Flows

## Backend API

### Subscriber Registration

A Client sends a subscriber registration request. The server processes the request and sends a response containing the Subscriber id.

### Establishing Connection

A Device sends a connection request containing a Subscriber id. The server processes the request, opens a Connection and assigns it to the Subscriber.

### Publishing Notifications

A Client sends a Notification publish request with the content and subscriber id. The server processes the request and sends a notification to all Subscriber's devices with active Connection

## Mobile Client

### Establishing Connection

On app startup, the Device sends a Subscription request with a Subscriber id. The server processes the request and opens a connection. Whenever necessary, the Device should re-establish the connection.
