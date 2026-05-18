## ADDED Requirements

### Requirement: MessageHub mapped at /hubs/messages
The API SHALL expose a SignalR hub at `/hubs/messages` accepting WebSocket connections. The hub SHALL require JWT bearer authentication using the same scheme as the REST API.

#### Scenario: Authenticated client connects
- **WHEN** a client connects to `/hubs/messages` with a valid JWT access token
- **THEN** the connection is established and the client is joined to SignalR groups for all conversations they participate in

#### Scenario: Unauthenticated client connects
- **WHEN** a client connects to `/hubs/messages` without a valid JWT
- **THEN** the connection is rejected with HTTP 401

#### Scenario: Token passed as query parameter
- **WHEN** a client connects with `?access_token=<jwt>` query parameter
- **THEN** the token is extracted and validated using the existing JWT bearer configuration

### Requirement: Group-per-conversation membership
On connection, the hub SHALL query all conversations the user participates in and join the connection to a SignalR group named `conversation-{conversationId}` for each one.

#### Scenario: User with 3 conversations connects
- **WHEN** a user participating in 3 conversations connects to the hub
- **THEN** the connection is added to 3 groups: `conversation-{id1}`, `conversation-{id2}`, `conversation-{id3}`

#### Scenario: User with no conversations connects
- **WHEN** a user with no conversations connects to the hub
- **THEN** the connection is established but added to no groups

#### Scenario: User disconnects
- **WHEN** a connected user disconnects
- **THEN** SignalR automatically removes the connection from all groups (no custom cleanup required)

### Requirement: Join group for newly created conversation
The hub SHALL expose a `JoinConversation(conversationId)` method that allows a connected client to join the group for a conversation they participate in. The hub SHALL verify that the caller is a participant before joining.

#### Scenario: Participant joins new conversation group
- **WHEN** a connected client calls `JoinConversation` with a conversation ID they participate in
- **THEN** the connection is added to group `conversation-{conversationId}`

#### Scenario: Non-participant attempts to join
- **WHEN** a connected client calls `JoinConversation` with a conversation ID they do not participate in
- **THEN** a HubException is thrown and the connection is NOT added to the group

### Requirement: ReceiveMessage client event
The hub SHALL broadcast a `ReceiveMessage` event to all connections in a conversation group when a new message is sent. The payload SHALL be a `MessageDto`.

#### Scenario: Message sent in conversation
- **WHEN** a message is persisted in conversation X
- **THEN** all connections in group `conversation-{X}` receive a `ReceiveMessage` event with the `MessageDto` payload

#### Scenario: Disconnected participant
- **WHEN** a message is sent and one participant is not connected
- **THEN** the connected participant receives the event; the disconnected participant receives nothing (they use REST polling as fallback)

### Requirement: MessageRead client event
The hub SHALL broadcast a `MessageRead` event to all connections in a conversation group when a message is marked as read. The payload SHALL include `messageId`, `readByProfileId`, and `readAt`.

#### Scenario: Message marked as read
- **WHEN** a user marks message M as read in conversation X
- **THEN** all connections in group `conversation-{X}` receive a `MessageRead` event with `{ messageId: M, readByProfileId, readAt }`

### Requirement: Typing indicator
The hub SHALL expose a `SendTyping(conversationId)` method that broadcasts a `UserTyping` event to other participants in the conversation group. The event SHALL NOT be sent back to the caller. No data SHALL be persisted.

#### Scenario: User starts typing
- **WHEN** a connected client calls `SendTyping` with a valid conversation ID
- **THEN** other connections in group `conversation-{conversationId}` receive a `UserTyping` event with `{ conversationId, profileId }`
- **AND** the caller's connection does NOT receive the event

#### Scenario: Non-participant attempts typing
- **WHEN** a connected client calls `SendTyping` with a conversation ID they do not participate in
- **THEN** a HubException is thrown and no event is broadcast

### Requirement: Hub stays thin
The hub SHALL NOT contain business logic. Message persistence, validation, and authorization for write operations SHALL remain in `MessagingService`. The hub is responsible only for connection lifecycle, group management, and typing relay.

#### Scenario: Hub method count
- **WHEN** the hub is inspected
- **THEN** it exposes only `JoinConversation` and `SendTyping` as client-callable methods (message sending and read marking go through the REST API, which triggers broadcasts via `IHubContext`)
