## Why

Clients and providers have no way to communicate through the MVC interface despite `IMessagingService` being fully implemented in the BLL layer with conversation creation, message sending, read tracking, and unread counts. The SignalR `MessageHub` exists for API/SPA clients, but MVC users have zero access to messaging. This is a critical gap — booking coordination, questions about service listings, and post-booking follow-up all require direct communication between participants.

## What Changes

- Add a `MessagingController` in the Client MVC area with conversation list, conversation detail, send message, and create conversation actions
- Add a conversation list page at `/Client/Messages` showing all conversations with participant name, last message preview, and unread count
- Add a conversation detail/chat page at `/Client/Messages/{id}` displaying the message thread with a send message form
- Add a "Message Provider" button on the booking details page to start or resume a conversation linked to a booking
- Add an unread message count badge in the Client area navigation bar
- Add ViewModels, mappers, and localization keys for all new messaging UI

## Capabilities

### New Capabilities

- `client-conversation-list`: Paginated conversation list showing participant names, last message preview, unread counts, and booking links
- `client-conversation-detail`: Conversation message thread view with send message form, read receipt marking, and pagination

### Modified Capabilities

- `client-booking-management-mvc`: Booking details page gains a "Message Provider" / "Message Client" button that creates or navigates to the booking-linked conversation

## Impact

- **Controllers**: New `MessagingController` in `Areas/Client/Controllers/` with `Index`, `Details` GET, `SendMessage` POST, `Create` POST actions
- **Views**: 3 new views (Messages/Index, Messages/Details, Messages/_UnreadBadge partial) + update to Bookings/Details
- **ViewModels**: 4 new ViewModels (ConversationListViewModel, ConversationDetailViewModel, SendMessageViewModel, ConversationListItemViewModel)
- **Mappers**: New `MessagingViewModelMapper` extension methods in Client area
- **Resources**: New resx keys in SharedResource.resx (EN) and SharedResource.et.resx (ET) for messaging UI strings
- **Layout**: `_ClientLayout.cshtml` updated with Messages nav link and unread badge
- **Dependencies**: `IMessagingService` already exists — no BLL/DAL changes needed
