## Context

AgriMarket is a .NET 10 agricultural services marketplace with a clean architecture: Domain entities → DAL (EF Core + PostgreSQL) → BLL (services + DTOs) → API (REST controllers). The messaging domain model is already in place — `Conversation`, `ConversationParticipant`, `Message`, and `MessageRead` entities are defined, configured in `AppDbContext` with proper relationships and indexes, and migrated to the database. No service, DTO, repository, or controller layer exists for messaging yet.

Existing patterns established by `BookingService`/`BookingsController` provide the blueprint: primary constructor DI, manual DTO mapping, `IUnitOfWork` for persistence, `BusinessRuleException` for validation, `ProblemDetails` for API errors, JWT `profileId` claim for authorization.

## Goals / Non-Goals

**Goals:**
- Provide a complete REST API for in-app messaging between users
- Support conversation creation (optionally linked to a booking), message sending, and read tracking
- Enforce authorization — only conversation participants can access conversations and messages
- Achieve 80%+ test coverage with unit tests (service layer) and integration tests (API layer)
- Follow all existing codebase patterns and conventions

**Non-Goals:**
- Real-time messaging via SignalR/WebSockets (future enhancement)
- File/image attachments in messages
- Group conversations with more than 2 participants (model supports it, but API scoped to 2-party for now)
- Message editing or deletion
- Push notifications
- Full-text search across messages

## Decisions

### 1. Conversation creation requires exactly 2 participant profile IDs

**Decision**: `CreateConversationDto` accepts a list of participant profile IDs (must include the caller). The service validates that exactly 2 participants are provided and that both profiles exist.

**Rationale**: The domain model supports N participants, but the MVP use case is provider-client communication. Constraining to 2 simplifies authorization logic and prevents abuse. The model is forward-compatible — removing the constraint later requires no schema changes.

**Alternative considered**: Auto-creating conversations from bookings. Rejected because users may want to message before booking, and not all conversations are booking-related.

### 2. Duplicate conversation prevention

**Decision**: When creating a conversation without a booking, the service checks if a conversation already exists between the same 2 participants (without a booking link). If so, it returns the existing conversation instead of creating a duplicate.

**Rationale**: Prevents users from accidentally creating multiple conversation threads with the same person. Booking-linked conversations are always created fresh since each booking is a distinct context.

### 3. Authorization enforced at service layer, not controller

**Decision**: `MessagingService` methods accept the caller's `profileId` and throw `UnauthorizedAccessException` if the caller is not a participant. The controller translates this to HTTP 403.

**Rationale**: Consistent with existing patterns (`BookingService` validates ownership). Keeps authorization logic testable at the unit level and prevents bypass from alternative consumers (e.g., MVC web controllers).

### 4. Paginated messages within conversations

**Decision**: `GetConversationAsync` returns conversation metadata + paginated messages (newest first, configurable page/pageSize). Conversation list endpoint returns summaries with last message and unread count.

**Rationale**: Conversations can grow large. Pagination prevents loading thousands of messages. Newest-first matches chat UX patterns. Summary endpoint enables efficient inbox rendering.

### 5. Read tracking via explicit endpoint

**Decision**: A separate `POST /api/v1/messages/{id}/read` endpoint marks a single message as read for the caller. The service creates a `MessageRead` record (idempotent — duplicate calls are no-ops via unique index).

**Rationale**: Explicit read marking gives the client control over when messages are considered read (e.g., on scroll into view). Batch "mark all read" can be added later without breaking this contract.

### 6. Repository layer for complex queries only

**Decision**: Add `IConversationRepository` with methods for listing conversations with last message/unread count, and finding existing conversations between participants. Use generic `IRepository<Message>` and `IRepository<MessageRead>` for simple CRUD — no dedicated message repository needed.

**Rationale**: Only conversations require complex multi-join queries. Messages and MessageReads are accessed through simple operations (add, find by predicate) that the generic repository handles well. Avoids unnecessary abstraction.

## Risks / Trade-offs

- **[No real-time updates]** → Users must poll or refresh to see new messages. Acceptable for MVP; SignalR can be layered on later without API changes.
- **[2-participant limit]** → Limits future group messaging. Mitigation: domain model already supports N participants; only the service validation needs to change.
- **[No message deletion]** → Sent messages are permanent. Acceptable for an agricultural services marketplace where conversation history has value for dispute resolution.
- **[Unread count query performance]** → Counting unread messages requires joining Messages → MessageReads per conversation. Mitigation: `MessageRead` has a unique index on (MessageId, UserProfileId) and Messages has an index on SentAt. For MVP volumes this is sufficient; a denormalized counter can be added if needed.

## Future: Real-time Messaging (SignalR)

The next planned change (`add-realtime-messaging`) should layer SignalR on top of this REST API:

- **MessageHub** — `Hub` class managing conversation group subscriptions. Clients join a group per conversation on connect.
- **Server-to-client push** — After `MessagingService.SendMessageAsync` persists a message, inject `IHubContext<MessageHub>` to push the `MessageDto` to the conversation group. The REST endpoint remains unchanged.
- **Live unread count** — Push updated unread count to the recipient after each new message.
- **Typing indicators** — Optional client-to-server-to-client relay through the hub (no persistence needed).
- **No API changes required** — All existing REST endpoints stay as-is. SignalR adds a parallel real-time channel; clients that don't support WebSockets fall back to polling the existing endpoints.
- **Key integration point** — `MessagingService` is the single place where hub notifications should be triggered, keeping real-time logic out of controllers.
