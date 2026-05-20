## 1. ViewModels and Mappers

- [ ] 1.1 Create `ConversationListViewModel` (list of `ConversationListItemViewModel`, current page, total pages, page size)
- [ ] 1.2 Create `ConversationListItemViewModel` (ConversationId, ParticipantName, LastMessagePreview, UnreadCount, BookingId?, LastActivityAt)
- [ ] 1.3 Create `ConversationDetailViewModel` (ConversationId, ParticipantName, Messages list, SendMessageForm, BookingId?, CurrentPage, TotalPages)
- [ ] 1.4 Create `SendMessageViewModel` (ConversationId, Content with [Required] validation)
- [ ] 1.5 Create `MessagingViewModelMapper` with extension methods to map `ConversationDto` to `ConversationListItemViewModel` and `ConversationDto` (with messages) to `ConversationDetailViewModel`

> **GIT COMMIT:** `feat: add messaging ViewModels and mapper`

## 2. Controller

- [ ] 2.1 Create `MessagingController` in `Areas/Client/Controllers/` with constructor injection of `IMessagingService`, `IStringLocalizer<SharedResource>`, and user profile resolution
- [ ] 2.2 Add `Index` GET action — call `GetConversationsAsync(callerProfileId, page, pageSize)`, map to `ConversationListViewModel`, return view
- [ ] 2.3 Add `Details` GET action — call `MarkAllAsReadAsync(callerProfileId, conversationId)` then `GetConversationAsync(callerProfileId, conversationId, page, pageSize)`, map to `ConversationDetailViewModel`, return view
- [ ] 2.4 Add `SendMessage` POST action — validate model, call `SendMessageAsync(callerProfileId, conversationId, content)`, redirect to `Details` (PRG pattern)
- [ ] 2.5 Add `Create` POST action — accept participant profile ID and optional booking ID, call `CreateConversationAsync(callerProfileId, participantProfileIds, bookingId)`, redirect to `Details` of the new conversation

> **GIT COMMIT:** `feat: implement MessagingController with conversation actions`

## 3. Views

- [ ] 3.1 Create `Areas/Client/Views/Messaging/Index.cshtml` — conversation list with participant name, last message preview, unread badge, booking link, pagination controls, empty state
- [ ] 3.2 Create `Areas/Client/Views/Messaging/Details.cshtml` — message thread (chronological, own messages styled differently), send message form at bottom, participant name header, pagination for older messages
- [ ] 3.3 Create `Areas/Client/Views/Messaging/_UnreadBadge.cshtml` — partial view rendering unread count badge (hidden when zero)

> **GIT COMMIT:** `feat: add messaging conversation and detail views`

## 4. Update Existing Views

- [ ] 4.1 Update `Areas/Client/Views/Bookings/Details.cshtml` — add "Message Provider" / "Message Client" button that POSTs to `/Client/Messages/Create` with participant profile ID and booking ID
- [ ] 4.2 Update `Areas/Client/Views/Shared/_ClientLayout.cshtml` — add "Messages" link in navigation bar with unread badge partial
- [ ] 4.3 Add unread count data to layout — use a ViewComponent, action filter, or base controller to call `GetUnreadCountAsync(profileId)` and make the count available to the layout partial on every page

> **GIT COMMIT:** `feat: integrate messaging into bookings and navigation`

## 5. Localization

- [ ] 5.1 Add EN resx keys to `SharedResource.resx`: Messages, Conversations, SendMessage, MessagePlaceholder, NoConversations, NoMessages, MessageProvider, MessageClient, UnreadMessages, ViewBooking, NewMessage, Send, Back
- [ ] 5.2 Add ET resx keys to `SharedResource.et.resx`: corresponding Estonian translations for all keys added in 5.1

> **GIT COMMIT:** `feat: add messaging localization strings`

## 6. Tests

- [ ] 6.1 Add unit tests for `MessagingController.Index` — verifies `GetConversationsAsync` is called, ViewModel is correctly mapped, empty list returns view with empty state
- [ ] 6.2 Add unit tests for `MessagingController.Details` — verifies `MarkAllAsReadAsync` is called, `GetConversationAsync` is called, non-participant access is denied, non-existent conversation returns NotFound
- [ ] 6.3 Add unit tests for `MessagingController.SendMessage` — verifies `SendMessageAsync` is called, redirect to Details on success, validation error on empty content, non-participant access is denied
- [ ] 6.4 Add unit tests for `MessagingController.Create` — verifies `CreateConversationAsync` is called with correct parameters, redirect to Details of new conversation
- [ ] 6.5 Add unit tests for `MessagingViewModelMapper` — verifies correct mapping from DTOs to ViewModels, truncation of last message preview, null handling

> **GIT COMMIT:** `test: add MessagingController and mapper unit tests`
