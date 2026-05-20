## Purpose

E2E tests for conversations, message sending, and unread counts.

## Requirements

### Requirement: Conversation list test
The test suite SHALL verify the conversation list at `/Client/Messaging`.

#### Scenario: Conversations are listed
- **WHEN** a user with conversations navigates to `/Client/Messaging`
- **THEN** the page displays conversations with participant info and last message preview

### Requirement: Send message test
The test suite SHALL verify sending messages in a conversation.

#### Scenario: Successful message send
- **WHEN** a user opens a conversation, types a message, and submits
- **THEN** the message appears in the conversation thread

#### Scenario: Send empty message
- **WHEN** a user attempts to send an empty message
- **THEN** the message is not sent (form validation or no action)

### Requirement: Conversation detail test
The test suite SHALL verify the conversation detail page.

#### Scenario: Messages are displayed
- **WHEN** a user opens a conversation
- **THEN** all messages in the conversation are displayed in chronological order with sender info and timestamps

### Requirement: Cross-user messaging test
The test suite SHALL verify that messages sent by one user are visible to the other.

#### Scenario: Provider sees farmer's message
- **WHEN** farmer sends a message in a conversation
- **AND** provider opens the same conversation
- **THEN** the farmer's message is visible to the provider

### Requirement: Unread message count test
The test suite SHALL verify unread message indicators.

#### Scenario: Unread badge updates
- **WHEN** a user receives a new message
- **AND** the user navigates to the messaging page
- **THEN** the conversation with the unread message shows an unread indicator
