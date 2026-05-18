## ADDED Requirements

### Requirement: IConversationRepository interface
The DAL SHALL provide `IConversationRepository` extending repository capabilities with conversation-specific query methods. It SHALL be registered as scoped in `DalServiceExtensions.AddDal()`.

#### Scenario: DI resolves IConversationRepository
- **WHEN** a service requests `IConversationRepository` from DI
- **THEN** the `EfConversationRepository` implementation is injected

### Requirement: Find existing conversation between participants
`IConversationRepository` SHALL provide `FindBetweenParticipantsAsync(profileId1, profileId2)` that returns an existing conversation (without a booking link) between the two given profiles, or null if none exists.

#### Scenario: Existing conversation found
- **WHEN** `FindBetweenParticipantsAsync` is called for 2 profiles with an existing non-booking conversation
- **THEN** the conversation is returned with its participants loaded

#### Scenario: No existing conversation
- **WHEN** `FindBetweenParticipantsAsync` is called for 2 profiles with no shared conversation
- **THEN** null is returned

#### Scenario: Booking conversations excluded
- **WHEN** `FindBetweenParticipantsAsync` is called for 2 profiles who share only a booking-linked conversation
- **THEN** null is returned (booking conversations are excluded from duplicate detection)

### Requirement: List conversations with summaries
`IConversationRepository` SHALL provide `ListWithSummariesAsync(profileId, page, pageSize)` that returns a paginated list of conversations the user participates in, including: last message (content, sender profile ID, sent time), unread message count for the caller, and other participant details. Results SHALL be ordered by last message timestamp descending (conversations with no messages ordered by creation date).

#### Scenario: Paginated conversation list
- **WHEN** `ListWithSummariesAsync` is called with `page: 1, pageSize: 10`
- **THEN** at most 10 conversations are returned, ordered by most recent activity

#### Scenario: Unread count calculation
- **WHEN** a conversation has 5 messages and the caller has read 3
- **THEN** the unread count for that conversation is 2 (sender's own messages are excluded from unread calculation)

#### Scenario: Last message populated
- **WHEN** a conversation has messages
- **THEN** the summary includes the most recent message's content, sender profile ID, and sent timestamp

#### Scenario: Empty conversation
- **WHEN** a conversation has no messages yet
- **THEN** the last message fields are null and the conversation appears at the end (sorted by creation date)

### Requirement: Get conversation with participant details
`IConversationRepository` SHALL provide `GetWithParticipantsAsync(conversationId)` that returns a conversation with its participants and their user profile details (first name, last name) eagerly loaded.

#### Scenario: Conversation found with participants
- **WHEN** `GetWithParticipantsAsync` is called with a valid conversation ID
- **THEN** the conversation is returned with participants and their UserProfile navigation properties loaded

#### Scenario: Conversation not found
- **WHEN** `GetWithParticipantsAsync` is called with a non-existent ID
- **THEN** null is returned

### Requirement: Get paginated messages for conversation
`IConversationRepository` SHALL provide `GetMessagesAsync(conversationId, callerProfileId, page, pageSize)` that returns paginated messages for a conversation, newest first, with each message including whether the caller has read it (via left join to `MessageRead`).

#### Scenario: Paginated messages returned
- **WHEN** `GetMessagesAsync` is called with `page: 1, pageSize: 50`
- **THEN** at most 50 messages are returned, ordered by `SentAt` descending

#### Scenario: Read status per message
- **WHEN** messages are returned and the caller has read some of them
- **THEN** each message includes a boolean indicating whether a `MessageRead` record exists for that message and caller

### Requirement: Count total unread messages
`IConversationRepository` SHALL provide `CountUnreadAsync(profileId)` that returns the total number of unread messages across all conversations the user participates in. A message is unread if: (1) the user is a participant, (2) the user is not the sender, and (3) no `MessageRead` record exists for that message and user.

#### Scenario: Correct unread count
- **WHEN** `CountUnreadAsync` is called for a user participating in 3 conversations with 2, 1, and 0 unread messages respectively
- **THEN** the result is 3

#### Scenario: Own messages excluded
- **WHEN** a user sent 5 messages in a conversation and has not "read" any messages
- **THEN** those 5 sent messages are NOT counted as unread for the sender

### Requirement: Check participant membership
`IConversationRepository` SHALL provide `IsParticipantAsync(conversationId, profileId)` returning a boolean indicating whether the given profile is a participant of the given conversation.

#### Scenario: User is participant
- **WHEN** `IsParticipantAsync` is called for a user who is a participant
- **THEN** the result is true

#### Scenario: User is not participant
- **WHEN** `IsParticipantAsync` is called for a user who is not a participant
- **THEN** the result is false

### Requirement: Repository uses EF Core with AppDbContext
`EfConversationRepository` SHALL use `AppDbContext` via constructor injection and leverage EF Core LINQ for all queries. Complex queries SHALL use projections to avoid loading unnecessary navigation properties.

#### Scenario: Efficient query for summaries
- **WHEN** `ListWithSummariesAsync` is called
- **THEN** the generated SQL uses projections (SELECT specific columns) rather than loading full entity graphs
