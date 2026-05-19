## 1. ViewModels

- [ ] 1.1 Create `ConversationListViewModel` (list of `ConversationListItemViewModel`, current page, total pages, page size)
- [ ] 1.2 Create `ConversationListItemViewModel` (ConversationId, ParticipantName, LastMessagePreview, UnreadCount, BookingId?, LastActivityAt)
- [ ] 1.3 Create `ConversationDetailViewModel` (ConversationId, ParticipantName, Messages list, SendMessageForm, BookingId?, CurrentPage, TotalPages)
- [ ] 1.4 Create `SendMessageViewModel` (ConversationId, Content with [Required] validation)

## 2. Mappers

- [ ] 2.1 Create `MessagingViewModelMapper` with extension methods to map `ConversationDto` to `ConversationListItemViewModel` and `ConversationDto` (with messages) to `ConversationDetailViewModel`

## 3. Controller

- [ ] 3.1 Create `MessagingController` in `Areas/Client/Controllers/` with constructor injection of `IMessagingService`, `IStringLocalizer<SharedResource>`, and user profile resolution
- [ ] 3.2 Add `Index` GET action — call `GetConversationsAsync(callerProfileId, page, pageSize)`, map to `ConversationListViewModel`, return view
- [ ] 3.3 Add `Details` GET action — call `MarkAllAsReadAsync(callerProfileId, conversationId)` then `GetConversationAsync(callerProfileId, conversationId, page, pageSize)`, map to `ConversationDetailViewModel`, return view
- [ ] 3.4 Add `SendMessage` POST action — validate model, call `SendMessageAsync(callerProfileId, conversationId, content)`, redirect to `Details` (PRG pattern)
- [ ] 3.5 Add `Create` POST action — accept participant profile ID and optional booking ID, call `CreateConversationAsync(callerProfileId, participantProfileIds, bookingId)`, redirect to `Details` of the new conversation

## 4. Views

- [ ] 4.1 Create `Areas/Client/Views/Messaging/Index.cshtml` — conversation list with participant name, last message preview, unread badge, booking link, pagination controls, empty state
- [ ] 4.2 Create `Areas/Client/Views/Messaging/Details.cshtml` — message thread (chronological, own messages styled differently), send message form at bottom, participant name header, pagination for older messages
- [ ] 4.3 Create `Areas/Client/Views/Messaging/_UnreadBadge.cshtml` — partial view rendering unread count badge (hidden when zero)

## 5. Update Existing Views

- [ ] 5.1 Update `Areas/Client/Views/Bookings/Details.cshtml` — add "Message Provider" / "Message Client" button that POSTs to `/Client/Messages/Create` with participant profile ID and booking ID
- [ ] 5.2 Update `Areas/Client/Views/Shared/_ClientLayout.cshtml` — add "Messages" link in navigation bar with unread badge partial
- [ ] 5.3 Add unread count data to layout — use a ViewComponent, action filter, or base controller to call `GetUnreadCountAsync(profileId)` and make the count available to the layout partial on every page

## 6. Localization

- [ ] 6.1 Add EN resx keys to `SharedResource.resx`: Messages, Conversations, SendMessage, MessagePlaceholder, NoConversations, NoMessages, MessageProvider, MessageClient, UnreadMessages, ViewBooking, NewMessage, Send, Back
- [ ] 6.2 Add ET resx keys to `SharedResource.et.resx`: corresponding Estonian translations for all keys added in 6.1

## 7. Tests

- [ ] 7.1 Add unit tests for `MessagingController.Index` — verifies `GetConversationsAsync` is called, ViewModel is correctly mapped, empty list returns view with empty state
- [ ] 7.2 Add unit tests for `MessagingController.Details` — verifies `MarkAllAsReadAsync` is called, `GetConversationAsync` is called, non-participant access is denied, non-existent conversation returns NotFound
- [ ] 7.3 Add unit tests for `MessagingController.SendMessage` — verifies `SendMessageAsync` is called, redirect to Details on success, validation error on empty content, non-participant access is denied
- [ ] 7.4 Add unit tests for `MessagingController.Create` — verifies `CreateConversationAsync` is called with correct parameters, redirect to Details of new conversation
- [ ] 7.5 Add unit tests for `MessagingViewModelMapper` — verifies correct mapping from DTOs to ViewModels, truncation of last message preview, null handling
