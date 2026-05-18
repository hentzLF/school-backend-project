## 1. JWT Authentication for SignalR

- [x] 1.1 Add `JwtBearerEvents.OnMessageReceived` handler in auth configuration to extract `access_token` from query string for hub requests
- [x] 1.2 Verify hub rejects unauthenticated connections with HTTP 401
- [x] 1.3 Commit: `feat: add JWT authentication support for SignalR hub`

## 2. MessageHub

- [x] 2.1 Create `AgriMarket.Api/Hubs/MessageHub.cs` with `[Authorize]` attribute
- [x] 2.2 Implement `OnConnectedAsync` — query caller's conversations via `IConversationRepository` and join each group (`conversation-{id}`)
- [x] 2.3 Implement `JoinConversation(Guid conversationId)` — verify participation via `IConversationRepository.IsParticipantAsync`, join group or throw `HubException`
- [x] 2.4 Implement `SendTyping(Guid conversationId)` — verify participation, broadcast `UserTyping` event to group excluding caller
- [x] 2.5 Commit: `feat: add MessageHub with group management and typing indicators`

## 3. Hub Registration

- [x] 3.1 Map hub route in `Program.cs`: `app.MapHub<MessageHub>("/hubs/messages")`
- [x] 3.2 Add `builder.Services.AddSignalR()` in `Program.cs`
- [x] 3.3 Commit: `feat: register SignalR and map MessageHub route`

## 4. Service Layer Integration

- [x] 4.1 Add `IHubContext<MessageHub>` to `MessagingService` constructor injection
- [x] 4.2 Broadcast `ReceiveMessage` event to conversation group after `SendMessageAsync` persists the message
- [x] 4.3 Broadcast `MessageRead` event to conversation group after `MarkAsReadAsync` creates a new read receipt (skip broadcast on idempotent re-read)
- [x] 4.4 Commit: `feat: broadcast real-time events from MessagingService via SignalR`

## 5. Tests

- [x] 5.1 Unit test: `MessageHub.OnConnectedAsync` joins correct groups
- [x] 5.2 Unit test: `MessageHub.JoinConversation` rejects non-participants
- [x] 5.3 Unit test: `MessageHub.SendTyping` broadcasts to group excluding caller
- [x] 5.4 Unit test: `MessagingService.SendMessageAsync` broadcasts `ReceiveMessage` after persist
- [x] 5.5 Unit test: `MessagingService.MarkAsReadAsync` broadcasts `MessageRead` on new read, skips on idempotent re-read
- [x] 5.6 Integration test: Connect to hub with valid JWT, verify connection accepted
- [x] 5.7 Integration test: Connect to hub without JWT, verify HTTP 401
- [x] 5.8 Commit: `test: add unit and integration tests for SignalR messaging hub`
