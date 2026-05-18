# messaging-service Specification

## Purpose
TBD - created by archiving change add-messaging-functionality. Update Purpose after archive.
## Requirements
### Requirement: IMessagingService interface in BLL
The BLL SHALL provide `IMessagingService` defining all messaging operations. The interface SHALL be registered as scoped in `BllServiceExtensions.AddBll()`.

#### Scenario: DI resolves IMessagingService
- **WHEN** a controller requests `IMessagingService` from DI
- **THEN** the `MessagingService` implementation is injected

### Requirement: Create conversation
`MessagingService.CreateConversationAsync(callerProfileId, dto)` SHALL create a new conversation with the given participants. It SHALL validate that exactly 2 participant profile IDs are provided, that the caller is one of them, and that both profiles exist.

#### Scenario: Valid creation with 2 participants
- **WHEN** `CreateConversationAsync` is called with 2 valid profile IDs including the caller
- **THEN** a `Conversation` is persisted with 2 `ConversationParticipant` records and a `ConversationDto` is returned

#### Scenario: Caller not in participant list
- **WHEN** `CreateConversationAsync` is called with participant IDs not including the caller's profile
- **THEN** `BusinessRuleException` is thrown

#### Scenario: Fewer than 2 participants
- **WHEN** `CreateConversationAsync` is called with 1 participant ID
- **THEN** `BusinessRuleException` is thrown

#### Scenario: More than 2 participants
- **WHEN** `CreateConversationAsync` is called with 3 participant IDs
- **THEN** `BusinessRuleException` is thrown

#### Scenario: Non-existent participant profile
- **WHEN** `CreateConversationAsync` is called with a profile ID that does not exist
- **THEN** `KeyNotFoundException` is thrown

### Requirement: Duplicate conversation prevention
When creating a conversation without a booking link between 2 participants who already have an existing non-booking conversation, the service SHALL return the existing conversation instead of creating a duplicate.

#### Scenario: Existing non-booking conversation between same participants
- **WHEN** `CreateConversationAsync` is called for 2 participants who already share a conversation with `BookingId = null`
- **THEN** the existing conversation is returned, no new records are created

#### Scenario: Booking-linked conversation always created fresh
- **WHEN** `CreateConversationAsync` is called with a booking ID, even if a conversation already exists between the participants
- **THEN** a new conversation is created and linked to the booking

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

### Requirement: List conversations for user
`MessagingService.GetConversationsAsync(callerProfileId, page, pageSize)` SHALL return a paginated list of conversation summaries for the caller, ordered by last message timestamp descending.

#### Scenario: User with conversations
- **WHEN** `GetConversationsAsync` is called for a user with 3 conversations
- **THEN** all 3 conversations are returned as summaries with last message and unread count

#### Scenario: User with no conversations
- **WHEN** `GetConversationsAsync` is called for a user with no conversations
- **THEN** an empty paginated result is returned

#### Scenario: Summary includes unread count
- **WHEN** `GetConversationsAsync` is called and a conversation has 3 messages not read by the caller
- **THEN** the summary for that conversation shows `unreadCount: 3`

### Requirement: Get single conversation with paginated messages
`MessagingService.GetConversationAsync(callerProfileId, conversationId, page, pageSize)` SHALL return conversation details with paginated messages (newest first). Only participants SHALL access it.

#### Scenario: Participant retrieves conversation
- **WHEN** `GetConversationAsync` is called by a participant
- **THEN** a `ConversationDto` is returned with paginated messages, newest first

#### Scenario: Non-participant retrieves conversation
- **WHEN** `GetConversationAsync` is called by a non-participant
- **THEN** `UnauthorizedAccessException` is thrown

#### Scenario: Non-existent conversation
- **WHEN** `GetConversationAsync` is called with a non-existent ID
- **THEN** `KeyNotFoundException` is thrown

#### Scenario: Messages include caller-specific read status
- **WHEN** messages are returned to a caller
- **THEN** each `MessageDto.IsRead` reflects whether the caller has a `MessageRead` record for that message

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

### Requirement: Get total unread count
`MessagingService.GetUnreadCountAsync(callerProfileId)` SHALL return the total number of unread messages across all conversations the caller participates in.

#### Scenario: User with unread messages
- **WHEN** `GetUnreadCountAsync` is called for a user with 5 unread messages across 2 conversations
- **THEN** the result is 5

#### Scenario: User with all messages read
- **WHEN** `GetUnreadCountAsync` is called for a user with no unread messages
- **THEN** the result is 0

#### Scenario: Own messages excluded from unread
- **WHEN** a user sends a message in a conversation
- **THEN** that message does not count as unread for the sender (sender's own messages are excluded from unread count)

### Requirement: DTO mapping in service layer
All service methods SHALL return DTOs, never domain entities. Mapping SHALL be implemented as private static methods within `MessagingService`, consistent with existing services like `BookingService`.

#### Scenario: Service returns DTOs
- **WHEN** any `IMessagingService` method returns data
- **THEN** the return type is a DTO from `AgriMarket.BLL.Dtos.Messaging`, not a domain entity

### Requirement: Persistence via IUnitOfWork
All write operations (create conversation, send message, mark as read) SHALL use `IUnitOfWork.SaveChangesAsync()` to persist changes, consistent with other BLL services.

#### Scenario: Message persisted via unit of work
- **WHEN** `SendMessageAsync` succeeds
- **THEN** `IUnitOfWork.SaveChangesAsync()` has been called

