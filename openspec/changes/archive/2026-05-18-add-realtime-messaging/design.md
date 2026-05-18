## Context

The messaging system has a complete REST API (conversations, messages, read tracking, unread counts) but no push mechanism. Clients must poll to detect new messages, which adds latency and server load. The domain model, service layer (`IMessagingService`/`MessagingService`), and repository layer (`IConversationRepository`/`EfConversationRepository`) are stable and tested.

ASP.NET Core includes SignalR in its shared framework — no additional NuGet package is required. The project already uses JWT bearer authentication, which SignalR supports natively via query string token transport.

## Goals / Non-Goals

**Goals:**
- Deliver new messages to connected participants in real time
- Broadcast read receipts so the sender sees when their message is read
- Support typing indicators (ephemeral, client-to-client)
- Authenticate hub connections using the same JWT scheme as REST endpoints
- Keep the hub thin — business logic stays in `MessagingService`

**Non-Goals:**
- Group chat (conversations remain 2-party)
- Message editing or deletion
- Online/offline presence tracking
- Push notifications for disconnected users (mobile push, email)
- Message delivery guarantees for offline clients (REST polling remains the fallback)
- Replacing any existing REST endpoints

## Decisions

### 1. Hub location and route

The hub will be a single `MessageHub` class in `AgriMarket.Api/Hubs/` mapped to `/hubs/messages`.

**Why:** A single hub keeps the surface small. The `/hubs/` prefix clearly separates WebSocket endpoints from REST routes. One hub is sufficient because all real-time events are messaging-related.

**Alternative considered:** Multiple hubs per event type (ChatHub, TypingHub). Rejected — unnecessary complexity for the current feature set.

### 2. Group-per-conversation

When a client connects, the hub joins them to a SignalR group for each conversation they participate in. Group name: `conversation-{conversationId}`.

**Why:** Groups are SignalR's built-in mechanism for targeted broadcasting. Mapping one group per conversation means `SendMessageAsync` can broadcast to `conversation-{id}` without querying who is online.

**Alternative considered:** User-level groups (one group per user). Rejected — would require the sender to know all participants and send individually, duplicating logic the conversation model already handles.

### 3. Broadcast from service layer via IHubContext

`MessagingService` will receive `IHubContext<MessageHub>` via constructor injection. After persisting a message or read receipt, it broadcasts the corresponding event to the conversation group.

**Why:** Keeps the controller and hub thin. The service already has the transaction boundary and the DTO — broadcasting from there avoids a second round-trip or event bus.

**Alternative considered:** Broadcast from the controller after calling the service. Rejected — scatters responsibility and means every caller of `SendMessageAsync` must remember to broadcast. Domain events via MediatR were also considered but add infrastructure for a single use case.

### 4. JWT authentication for hub

SignalR will reuse the existing JWT bearer scheme. The client sends the token as a query parameter (`?access_token=...`), and a `JwtBearerEvents.OnMessageReceived` handler extracts it for hub requests.

**Why:** This is the standard ASP.NET Core pattern for SignalR + JWT. No new auth infrastructure needed.

### 5. Typing indicators are ephemeral

Typing events are relayed directly from one client to the other via the hub — no persistence, no service call. The hub method `SendTyping(conversationId)` broadcasts to the group excluding the caller.

**Why:** Typing state is inherently transient. Persisting it would add write load with zero value. Hub-only handling keeps the service layer clean.

### 6. Client events

| Server → Client event | Payload | Trigger |
|---|---|---|
| `ReceiveMessage` | `MessageDto` | New message sent in a conversation |
| `MessageRead` | `{ messageId, readByProfileId, readAt }` | A message is marked as read |
| `UserTyping` | `{ conversationId, profileId }` | Another participant is typing |

### 7. Connection lifecycle

- **OnConnectedAsync**: Query all conversations for the caller's profile ID, join each conversation group.
- **OnDisconnectedAsync**: SignalR automatically removes the connection from all groups — no custom cleanup needed.

## Risks / Trade-offs

- **[Scaling]** SignalR groups are in-memory by default. If the app scales to multiple instances, a backplane (Redis) will be needed. → **Mitigation:** Single-instance deployment is sufficient for now. Redis backplane can be added later with one line of configuration — no code changes to hub or service.

- **[Token expiry]** JWT tokens sent via query string are validated only at connection time. A long-lived WebSocket may outlive the token. → **Mitigation:** Acceptable for MVP. Clients can reconnect when they refresh their access token. The REST API still validates per-request.

- **[Race condition]** A client might receive a real-time message before their initial REST load completes, causing a duplicate in the UI. → **Mitigation:** Frontend deduplication by message ID. Messages have stable GUIDs.

- **[Group membership staleness]** If a user is added to a new conversation while connected, they won't receive real-time messages until reconnect. → **Mitigation:** Conversations are always created by one of the 2 participants. The creator's hub connection can join the group immediately after creation. The other participant will join on their next connection or page refresh.
