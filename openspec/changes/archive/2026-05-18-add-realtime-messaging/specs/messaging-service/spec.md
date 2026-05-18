## MODIFIED Requirements

### Requirement: Send message
`MessagingService.SendMessageAsync(callerProfileId, conversationId, dto)` SHALL create a new message in the specified conversation. The caller MUST be a participant. After persisting the message, the service SHALL broadcast a `ReceiveMessage` event to the conversation's SignalR group via `IHubContext<MessageHub>`.

#### Scenario: Participant sends valid message
- **WHEN** `SendMessageAsync` is called by a conversation participant with non-empty content
- **THEN** a `Message` is persisted with `SenderProfileId = callerProfileId`, `SentAt = UTC now`, and a `MessageDto` is returned

#### Scenario: Real-time broadcast after send
- **WHEN** `SendMessageAsync` successfully persists a message
- **THEN** a `ReceiveMessage` event with the `MessageDto` payload is sent to all connections in group `conversation-{conversationId}`

#### Scenario: Non-participant attempts to send
- **WHEN** `SendMessageAsync` is called by a user who is not a participant
- **THEN** `UnauthorizedAccessException` is thrown and no broadcast occurs

#### Scenario: Empty content rejected
- **WHEN** `SendMessageAsync` is called with empty or whitespace-only content
- **THEN** `BusinessRuleException` is thrown and no broadcast occurs

#### Scenario: Non-existent conversation
- **WHEN** `SendMessageAsync` is called with a conversation ID that does not exist
- **THEN** `KeyNotFoundException` is thrown and no broadcast occurs

### Requirement: Mark message as read
`MessagingService.MarkAsReadAsync(callerProfileId, messageId)` SHALL create a `MessageRead` record for the caller. The caller MUST be a participant of the message's conversation. The operation SHALL be idempotent. After persisting the read receipt, the service SHALL broadcast a `MessageRead` event to the conversation's SignalR group via `IHubContext<MessageHub>`.

#### Scenario: First read
- **WHEN** `MarkAsReadAsync` is called for a message not yet read by the caller
- **THEN** a `MessageRead` record is created with `ReadAt = UTC now`

#### Scenario: Real-time broadcast after read
- **WHEN** `MarkAsReadAsync` successfully creates a new read receipt
- **THEN** a `MessageRead` event with `{ messageId, readByProfileId, readAt }` is sent to all connections in group `conversation-{conversationId}`

#### Scenario: Idempotent re-read does not broadcast
- **WHEN** `MarkAsReadAsync` is called for a message already read by the caller
- **THEN** no error is thrown, no duplicate record is created, and no broadcast is sent

#### Scenario: Non-participant attempts to mark read
- **WHEN** `MarkAsReadAsync` is called by a non-participant of the message's conversation
- **THEN** `UnauthorizedAccessException` is thrown and no broadcast occurs

#### Scenario: Non-existent message
- **WHEN** `MarkAsReadAsync` is called with a message ID that does not exist
- **THEN** `KeyNotFoundException` is thrown and no broadcast occurs
