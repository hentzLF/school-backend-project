## Why

The messaging system currently relies on REST polling — clients must repeatedly call `GET /conversations` and `GET /conversations/{id}` to detect new messages. This creates poor UX (visible latency) and unnecessary server load. Adding SignalR enables real-time push delivery of messages, read receipts, and typing indicators without modifying the existing REST API.

## What Changes

- Add a SignalR `MessageHub` that authenticates via JWT and groups clients by conversation
- Broadcast new messages to conversation participants in real time when `SendMessageAsync` completes
- Broadcast read receipts when `MarkAsReadAsync` completes
- Support typing indicator events (client-to-client, no persistence)
- Add SignalR NuGet package and wire up hub in `Program.cs`
- Inject `IHubContext<MessageHub>` into `MessagingService` to trigger broadcasts after writes

## Capabilities

### New Capabilities
- `realtime-messaging-hub`: SignalR hub for real-time message delivery, read receipts, and typing indicators over WebSocket connections

### Modified Capabilities
- `messaging-service`: Service layer gains real-time broadcast responsibility — after persisting a message or read receipt, it notifies connected clients via the hub

## Impact

- **New dependency**: `Microsoft.AspNetCore.SignalR` (included in ASP.NET Core shared framework, no extra NuGet needed)
- **API layer**: New `/hubs/messages` endpoint for WebSocket connections; all REST endpoints remain unchanged
- **BLL layer**: `MessagingService` receives `IHubContext<MessageHub>` to broadcast events
- **Auth**: Hub uses the same JWT bearer authentication as REST endpoints
- **Frontend**: Clients will connect via `@microsoft/signalr` JS client and listen for events
