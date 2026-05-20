# client-conversation-list Specification

## Purpose
TBD - created by archiving change client-messaging-mvc. Update Purpose after archive.
## Requirements
### Requirement: Client can view their conversations
The system SHALL provide a conversation list page at `/Client/Messages` that displays all conversations for the authenticated client via `IMessagingService.GetConversationsAsync(callerProfileId, page, pageSize)`. Each conversation entry SHALL show the other participant's name, last message preview (truncated), unread message count, and last activity timestamp. The list SHALL be ordered by most recent activity first and support pagination.

#### Scenario: View conversation list with conversations
- **WHEN** an authenticated client navigates to `/Client/Messages` and has existing conversations
- **THEN** the system displays a paginated list of conversations ordered by most recent activity first, each showing participant name, last message preview, unread count, and timestamp

#### Scenario: View conversation list with no conversations
- **WHEN** an authenticated client navigates to `/Client/Messages` and has no conversations
- **THEN** the system displays an empty-state message indicating no conversations exist

#### Scenario: Conversation with unread messages is visually distinguished
- **WHEN** a conversation in the list has unread messages (UnreadCount > 0)
- **THEN** the conversation entry displays the unread count badge and is visually highlighted

#### Scenario: Pagination on conversation list
- **WHEN** an authenticated client has more conversations than the page size
- **THEN** the system displays pagination controls to navigate between pages

### Requirement: Conversation list shows booking link when applicable
Each conversation entry SHALL indicate whether it is linked to a booking. If a BookingId is present, the conversation entry SHALL include a link or label referencing the associated booking.

#### Scenario: Conversation linked to a booking
- **WHEN** a conversation has an associated BookingId
- **THEN** the conversation entry displays a booking reference or link to the booking details page

#### Scenario: Conversation without a booking
- **WHEN** a conversation has no associated BookingId
- **THEN** the conversation entry does not display any booking reference

### Requirement: Unread message count displayed in navigation
The Client area navigation bar SHALL display the total unread message count for the authenticated client via `IMessagingService.GetUnreadCountAsync(profileId)`. The badge SHALL be hidden when the count is zero.

#### Scenario: Navigation shows unread badge when messages exist
- **WHEN** an authenticated client has unread messages (count > 0)
- **THEN** the navigation bar displays a badge with the unread message count next to the Messages link

#### Scenario: Navigation hides unread badge when no unread messages
- **WHEN** an authenticated client has zero unread messages
- **THEN** the navigation bar displays the Messages link without a badge

### Requirement: Messages link in client navigation
The Client area navigation SHALL include a link to `/Client/Messages` labeled appropriately in the current locale.

#### Scenario: Messages link visible in navigation
- **WHEN** an authenticated client views any page within the Client area
- **THEN** the navigation bar includes a "Messages" link pointing to `/Client/Messages`

