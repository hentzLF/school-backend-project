# client-conversation-detail Specification

## Purpose
TBD - created by archiving change client-messaging-mvc. Update Purpose after archive.
## Requirements
### Requirement: Client can view a conversation's messages
The system SHALL provide a conversation detail page at `/Client/Messages/Details/{id}` that displays the message thread for the specified conversation via `IMessagingService.GetConversationAsync(callerProfileId, conversationId, page, pageSize)`. Messages SHALL be displayed in chronological order (oldest first). The page SHALL show the other participant's name and support pagination for older messages.

#### Scenario: View conversation with messages
- **WHEN** an authenticated client navigates to `/Client/Messages/Details/{id}` for a conversation they participate in
- **THEN** the system displays the message thread in chronological order, each message showing sender name, content, and timestamp

#### Scenario: View conversation with no messages
- **WHEN** an authenticated client navigates to a conversation that has no messages yet
- **THEN** the system displays the conversation page with an empty message area and the send message form

#### Scenario: Pagination on message thread
- **WHEN** a conversation has more messages than the page size
- **THEN** the system displays pagination controls to load older messages

#### Scenario: Own messages are visually distinguished
- **WHEN** a message in the thread was sent by the authenticated client
- **THEN** the message is visually styled differently from messages sent by the other participant

### Requirement: Messages are marked as read when conversation is opened
The system SHALL call `IMessagingService.MarkAllAsReadAsync(callerProfileId, conversationId)` when a client opens a conversation detail page, marking all messages in that conversation as read for the authenticated client.

#### Scenario: Opening conversation marks messages as read
- **WHEN** an authenticated client navigates to `/Client/Messages/Details/{id}`
- **THEN** the system marks all unread messages in that conversation as read for the client

#### Scenario: Unread count updates after viewing conversation
- **WHEN** a client views a conversation that had unread messages and then navigates to the conversation list
- **THEN** the conversation's unread count shows as zero

### Requirement: Client can send a message in a conversation
The system SHALL provide a send message form on the conversation detail page. Submitting the form SHALL POST to `/Client/Messages/SendMessage` with the conversation ID and message content, calling `IMessagingService.SendMessageAsync(callerProfileId, conversationId, content)`. On success, the system SHALL redirect back to the conversation detail page (PRG pattern).

#### Scenario: Send a message successfully
- **WHEN** an authenticated client submits the send message form with non-empty content
- **THEN** the system sends the message via `IMessagingService.SendMessageAsync()` and redirects back to the conversation detail page showing the new message

#### Scenario: Send message with empty content
- **WHEN** an authenticated client submits the send message form with empty or whitespace-only content
- **THEN** the system rejects the submission with a validation error and does not send the message

#### Scenario: Send message to a conversation the client does not participate in
- **WHEN** a request is made to send a message to a conversation the authenticated client is not a participant of
- **THEN** the system denies access and does not send the message

### Requirement: Client can start a new conversation from booking details
The system SHALL provide a button on the booking details page (`/Client/Bookings/Details/{id}`) to start or resume a conversation with the other booking participant. Clicking the button SHALL POST to `/Client/Messages/Create` with the other participant's profile ID and the booking ID. The controller SHALL call `IMessagingService.CreateConversationAsync(callerProfileId, participantProfileIds, bookingId)` and redirect to the new conversation's detail page.

#### Scenario: Start a new conversation from booking
- **WHEN** an authenticated client clicks "Message Provider" on a booking details page and no conversation exists for that booking
- **THEN** the system creates a new conversation linked to the booking and redirects to the conversation detail page

#### Scenario: Resume existing conversation from booking
- **WHEN** an authenticated client clicks "Message Provider" on a booking details page and a conversation already exists for that booking
- **THEN** the system navigates to the existing conversation's detail page without creating a duplicate

### Requirement: Non-participant access is denied
The system SHALL deny access to a conversation detail page if the authenticated client is not a participant of the specified conversation. The system SHALL NOT reveal conversation content to non-participants.

#### Scenario: Access denied for non-participant
- **WHEN** an authenticated client navigates to `/Client/Messages/Details/{id}` for a conversation they are not a participant of
- **THEN** the system denies access and does not display the conversation content

#### Scenario: Access denied for non-existent conversation
- **WHEN** an authenticated client navigates to `/Client/Messages/Details/{id}` with a non-existent conversation ID
- **THEN** the system returns a not-found response

