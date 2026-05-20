## Context

The BLL layer provides a complete `IMessagingService` with conversation creation (2 participants, optional booking link), message sending, paginated conversation and message retrieval, read tracking (single message and bulk), and unread count queries. A SignalR `MessageHub` at `/hubs/messages` handles real-time messaging for API/SPA clients using JWT authentication. DTOs (`ConversationDto`, `MessageDto`, `SendMessageDto`, `ParticipantDto`, `UnreadCountDto`) are fully defined.

The Client MVC area at `AgriMarket.Web/Areas/Client/` has controllers for bookings, listings, and account management, but no messaging controller or views exist. The codebase follows a consistent pattern: ViewModels (no ViewBag/ViewData), `IStringLocalizer<SharedResource>` for i18n, manual mapper extension methods, and area routing at `/Client/{controller}/{action}`.

## Goals / Non-Goals

**Goals:**
- Provide a conversation list page where clients can see all their conversations with unread counts
- Provide a conversation detail page where clients can read messages and send replies
- Allow clients to start a conversation with a provider from the booking details page
- Mark messages as read when a conversation is opened
- Show unread message count in the Client area navigation bar
- Follow existing codebase patterns (ViewModels, Mappers, Localizer, area routing)

**Non-Goals:**
- Real-time SignalR updates in MVC views — use page refresh or simple form POST (SignalR is for API/SPA clients only)
- File attachments or media messages — text-only messaging
- Group chats — the service enforces exactly 2 participants per conversation
- Message search or filtering functionality
- Message editing or deletion
- Typing indicators or online status

## Decisions

### 1. Controller naming and routing

**Decision:** `MessagingController` in the Client area, routed at `/Client/Messages`.

**Rationale:** "Messages" is the user-facing noun (conversations contain messages). The controller name `MessagingController` aligns with `IMessagingService`. Route prefix `/Client/Messages` is concise and intuitive. Conversation list at `/Client/Messages`, detail at `/Client/Messages/Details/{id}`.

**Alternative considered:** `ConversationsController` — rejected because the user-facing concept is "Messages" (as seen in most messaging UIs), and having the URL say `/Conversations` is less natural for end users.

### 2. Send message as POST + redirect (PRG pattern)

**Decision:** Sending a message uses a standard form POST to `/Client/Messages/SendMessage` which redirects back to the conversation detail page.

**Rationale:** Standard ASP.NET MVC PRG (Post-Redirect-Get) pattern. Prevents double-submit on browser refresh. No JavaScript required. Consistent with the rest of the MVC codebase which avoids client-side frameworks.

**Alternative considered:** AJAX POST with partial page update — rejected because the codebase does not use AJAX patterns in other MVC views, and the added complexity is not justified for an assignment project.

### 3. Starting a conversation from booking details

**Decision:** A "Message Provider" / "Message Client" button on the booking details page POSTs to `/Client/Messages/Create` with the other participant's profile ID and the booking ID. The controller calls `IMessagingService.CreateConversationAsync()` and redirects to the new conversation's detail page. If a conversation already exists for that booking, the service handles deduplication or the controller navigates to the existing one.

**Rationale:** Tying conversations to bookings is the primary use case. The booking details page already has the participant information and booking ID needed to create a conversation.

### 4. Unread badge in navigation

**Decision:** Use a partial view `_UnreadBadge.cshtml` rendered via `IMessagingService.GetUnreadCountAsync()` in the `_ClientLayout.cshtml`. The controller sets the unread count in a shared layout mechanism (ViewComponent or a base controller that populates ViewData for layout use, then the partial reads it).

**Rationale:** A ViewComponent would be the cleanest approach (self-contained data fetching), but to stay consistent with existing codebase patterns, a simpler approach using a base controller or action filter that sets unread count on every request is acceptable. The badge shows the count and hides when zero.

**Alternative considered:** JavaScript polling for unread count — rejected as it introduces client-side complexity not present in the rest of the MVC app.

### 5. ViewModels

Four new ViewModels:
- `ConversationListViewModel` — paginated list of `ConversationListItemViewModel`, total count, current page
- `ConversationListItemViewModel` — conversation ID, participant name, last message preview (truncated), unread count, booking ID, last activity timestamp
- `ConversationDetailViewModel` — conversation info, paginated list of messages (mapped from `MessageDto`), send message form model, participant name
- `SendMessageViewModel` — conversation ID, message content (for POST binding)

### 6. Message ordering and pagination

**Decision:** Messages in conversation detail are displayed oldest-first (chronological order) with pagination loading older messages. The most recent page is shown by default.

**Rationale:** Chat interfaces universally show messages in chronological order. The user sees the latest messages first and can paginate backward to see older ones.

## Risks / Trade-offs

- **[Risk] No real-time updates in MVC** — Users must refresh the page to see new messages. This is acceptable for an assignment project and consistent with the stated non-goal. A simple "Refresh" link or auto-refresh meta tag could be added as a minor enhancement.
- **[Risk] Conversation deduplication** — If a client clicks "Message Provider" twice for the same booking, `CreateConversationAsync` may create duplicate conversations. Mitigation: Check if a conversation already exists for the booking before creating a new one, or handle the service-level exception gracefully.
- **[Trade-off] Unread badge requires a DB query on every page load** — `GetUnreadCountAsync` is called on every Client area page to populate the nav badge. For an assignment project this is acceptable. In production, caching or a lighter query would be warranted.
- **[Trade-off] No AJAX/JavaScript** — The messaging UX will feel less responsive than a real chat app. This is intentional per the non-goals, keeping the MVC layer simple and consistent with the rest of the codebase.
