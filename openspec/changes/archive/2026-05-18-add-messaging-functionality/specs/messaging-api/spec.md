## ADDED Requirements

### Requirement: List conversations
The API SHALL expose `GET /api/v1/conversations` returning a paginated list of conversation summaries for the authenticated user. Each summary SHALL include the conversation ID, other participant's name and profile ID, the last message (content, sender, timestamp), unread message count, and optional booking ID. Results SHALL be ordered by last message timestamp descending.

#### Scenario: Default pagination
- **WHEN** `GET /api/v1/conversations` is called with no query params by an authenticated user
- **THEN** the response returns HTTP 200 with `{ items, page: 1, pageSize: 20, totalCount }` where items are `ConversationSummaryDto` objects

#### Scenario: User has no conversations
- **WHEN** `GET /api/v1/conversations` is called by a user with no conversations
- **THEN** the response returns HTTP 200 with `{ items: [], page: 1, pageSize: 20, totalCount: 0 }`

#### Scenario: Unauthenticated request
- **WHEN** `GET /api/v1/conversations` is called without a valid JWT
- **THEN** the response returns HTTP 401

### Requirement: Get single conversation with messages
The API SHALL expose `GET /api/v1/conversations/{id}` returning conversation details with paginated messages (newest first). The response SHALL include conversation metadata, participant list, and a paginated messages array.

#### Scenario: Participant retrieves conversation
- **WHEN** `GET /api/v1/conversations/{id}` is called by a conversation participant
- **THEN** the response returns HTTP 200 with `ConversationDto` including paginated messages

#### Scenario: Non-participant retrieves conversation
- **WHEN** `GET /api/v1/conversations/{id}` is called by a user who is not a participant
- **THEN** the response returns HTTP 403 with a ProblemDetails body

#### Scenario: Non-existent conversation
- **WHEN** `GET /api/v1/conversations/{id}` is called with an ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body

#### Scenario: Paginated messages
- **WHEN** `GET /api/v1/conversations/{id}?page=2&pageSize=50` is called
- **THEN** the response returns the second page of messages, 50 per page, newest first

### Requirement: Create conversation
The API SHALL expose `POST /api/v1/conversations` accepting a `CreateConversationRequest` with a list of participant profile IDs and an optional booking ID. The caller's profile ID MUST be included in the participant list. Exactly 2 participants SHALL be required.

#### Scenario: Valid conversation creation
- **WHEN** `POST /api/v1/conversations` is called with 2 valid participant IDs (including caller) and no booking ID
- **THEN** the response returns HTTP 201 with the created `ConversationDto` and a Location header

#### Scenario: Conversation linked to booking
- **WHEN** `POST /api/v1/conversations` is called with a valid booking ID
- **THEN** the response returns HTTP 201 with a conversation linked to that booking

#### Scenario: Duplicate conversation returns existing
- **WHEN** `POST /api/v1/conversations` is called for 2 participants who already have a non-booking conversation
- **THEN** the response returns HTTP 200 with the existing conversation (no duplicate created)

#### Scenario: Caller not in participants
- **WHEN** `POST /api/v1/conversations` is called with participant IDs that do not include the caller
- **THEN** the response returns HTTP 400 with a ProblemDetails body

#### Scenario: Invalid participant count
- **WHEN** `POST /api/v1/conversations` is called with fewer or more than 2 participant IDs
- **THEN** the response returns HTTP 400 with a ProblemDetails body

#### Scenario: Non-existent participant profile
- **WHEN** `POST /api/v1/conversations` is called with a participant profile ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body

### Requirement: Send message
The API SHALL expose `POST /api/v1/conversations/{id}/messages` accepting a `SendMessageRequest` with message content. The sender SHALL be the authenticated user's profile.

#### Scenario: Participant sends message
- **WHEN** `POST /api/v1/conversations/{id}/messages` is called by a participant with valid content
- **THEN** the response returns HTTP 201 with the created `MessageDto`

#### Scenario: Non-participant sends message
- **WHEN** `POST /api/v1/conversations/{id}/messages` is called by a non-participant
- **THEN** the response returns HTTP 403 with a ProblemDetails body

#### Scenario: Empty message content
- **WHEN** `POST /api/v1/conversations/{id}/messages` is called with empty or whitespace-only content
- **THEN** the response returns HTTP 400 with a ProblemDetails body

#### Scenario: Message to non-existent conversation
- **WHEN** `POST /api/v1/conversations/{id}/messages` is called with a conversation ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body

### Requirement: Mark message as read
The API SHALL expose `POST /api/v1/messages/{id}/read` marking a message as read by the authenticated user. The operation SHALL be idempotent.

#### Scenario: Participant marks message as read
- **WHEN** `POST /api/v1/messages/{id}/read` is called by a conversation participant
- **THEN** the response returns HTTP 200 and a `MessageRead` record is created with the current timestamp

#### Scenario: Already-read message marked again
- **WHEN** `POST /api/v1/messages/{id}/read` is called for a message already marked read by this user
- **THEN** the response returns HTTP 200 (idempotent, no duplicate record created)

#### Scenario: Non-participant marks message as read
- **WHEN** `POST /api/v1/messages/{id}/read` is called by a user who is not a participant of the message's conversation
- **THEN** the response returns HTTP 403 with a ProblemDetails body

#### Scenario: Non-existent message
- **WHEN** `POST /api/v1/messages/{id}/read` is called with a message ID that does not exist
- **THEN** the response returns HTTP 404 with a ProblemDetails body

### Requirement: Get unread message count
The API SHALL expose `GET /api/v1/conversations/unread-count` returning the total number of unread messages across all conversations for the authenticated user.

#### Scenario: User with unread messages
- **WHEN** `GET /api/v1/conversations/unread-count` is called by a user with 5 unread messages
- **THEN** the response returns HTTP 200 with `{ unreadCount: 5 }`

#### Scenario: User with no unread messages
- **WHEN** `GET /api/v1/conversations/unread-count` is called by a user with all messages read
- **THEN** the response returns HTTP 200 with `{ unreadCount: 0 }`

### Requirement: ConversationSummaryDto shape
`ConversationSummaryDto` SHALL include: `id` (Guid), `bookingId` (Guid?), `otherParticipant` (object with `profileId` and `fullName`), `lastMessage` (object with `content`, `senderProfileId`, `sentAt`, or null if no messages), `unreadCount` (int), `createdAt` (DateTime).

#### Scenario: Summary includes all fields
- **WHEN** a conversation summary is returned
- **THEN** all specified fields are present and correctly populated

### Requirement: ConversationDto shape
`ConversationDto` SHALL include: `id` (Guid), `bookingId` (Guid?), `createdAt` (DateTime), `participants` (list of objects with `profileId` and `fullName`), `messages` (paginated `MessageDto` list).

#### Scenario: Conversation includes participant details
- **WHEN** a conversation detail is returned
- **THEN** the participants list includes profile ID and full name for each participant

### Requirement: MessageDto shape
`MessageDto` SHALL include: `id` (Guid), `conversationId` (Guid), `senderProfileId` (Guid), `senderName` (string), `content` (string), `sentAt` (DateTime), `isRead` (bool — whether the current user has read this message).

#### Scenario: Message read status is caller-specific
- **WHEN** a message is returned to user A who has read it
- **THEN** `isRead` is true for user A but would be false for user B who has not read it

### Requirement: API versioning and route convention
All messaging endpoints SHALL be under `api/v1/` using the same `ApiVersion("1")` and route pattern as existing controllers.

#### Scenario: Route matches convention
- **WHEN** messaging endpoints are called
- **THEN** they are accessible at `/api/v1/conversations/...` and `/api/v1/messages/...`
