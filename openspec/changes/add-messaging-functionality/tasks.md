## 1. DTOs

- [x] 1.1 Create `MessageDto` record in `AgriMarket.BLL/Dtos/Messaging/` (Id, ConversationId, SenderProfileId, SenderName, Content, SentAt, IsRead)
- [x] 1.2 Create `ConversationDto` record (Id, BookingId, CreatedAt, Participants list, Messages paginated)
- [x] 1.3 Create `ConversationSummaryDto` record (Id, BookingId, OtherParticipant, LastMessage, UnreadCount, CreatedAt)
- [x] 1.4 Create `CreateConversationDto` record (ParticipantProfileIds list, BookingId optional)
- [x] 1.5 Create `SendMessageDto` record (Content)
- [x] 1.6 Create `ParticipantDto` record (ProfileId, FullName)
- [x] 1.7 Create `LastMessageDto` record (Content, SenderProfileId, SentAt)
- [x] 1.8 Create `UnreadCountDto` record (UnreadCount)
- [x] 1.9 Run `dotnet build` to verify compilation
- [x] 1.10 Commit: `feat: add messaging DTOs`

## 2. Repository Layer

- [x] 2.1 Create `IConversationRepository` interface in `AgriMarket.BLL/Contracts/` with methods: FindBetweenParticipantsAsync, ListWithSummariesAsync, GetWithParticipantsAsync, GetMessagesAsync, CountUnreadAsync, IsParticipantAsync
- [x] 2.2 Create `EfConversationRepository` implementation in `AgriMarket.DAL/Repositories/` using AppDbContext with EF Core LINQ projections
- [x] 2.3 Register `IConversationRepository` → `EfConversationRepository` in `DalServiceExtensions.AddDal()`
- [x] 2.4 Run `dotnet build` to verify compilation
- [x] 2.5 Commit: `feat: add conversation repository with query methods`

## 3. Service Layer

- [x] 3.1 Create `IMessagingService` interface in `AgriMarket.BLL/Services/` with methods: CreateConversationAsync, SendMessageAsync, GetConversationsAsync, GetConversationAsync, MarkAsReadAsync, GetUnreadCountAsync
- [x] 3.2 Implement `MessagingService` with conversation creation (2-participant validation, duplicate prevention, booking link)
- [x] 3.3 Implement `SendMessageAsync` with participant authorization check and content validation
- [x] 3.4 Implement `GetConversationsAsync` with paginated summaries (last message, unread count, other participant)
- [x] 3.5 Implement `GetConversationAsync` with participant authorization and paginated messages (newest first, caller-specific read status)
- [x] 3.6 Implement `MarkAsReadAsync` with participant authorization and idempotent behavior
- [x] 3.7 Implement `GetUnreadCountAsync` excluding sender's own messages
- [x] 3.8 Add private static DTO mapping methods in MessagingService
- [x] 3.9 Register `IMessagingService` → `MessagingService` in `BllServiceExtensions.AddBll()`
- [x] 3.10 Run `dotnet build` to verify compilation
- [x] 3.11 Commit: `feat: add messaging service with authorization`

## 4. API Controller

- [x] 4.1 Create `ConversationsController` with route `api/v1/conversations`, ApiVersion("1"), inheriting ApiControllerBase
- [x] 4.2 Implement `POST /api/v1/conversations` — create conversation endpoint
- [x] 4.3 Implement `GET /api/v1/conversations` — list conversations with pagination
- [x] 4.4 Implement `GET /api/v1/conversations/{id}` — get single conversation with paginated messages
- [x] 4.5 Implement `POST /api/v1/conversations/{id}/messages` — send message endpoint
- [x] 4.6 Implement `POST /api/v1/messages/{id}/read` — mark message as read endpoint
- [x] 4.7 Implement `GET /api/v1/conversations/unread-count` — get total unread count
- [x] 4.8 Add `[ProducesResponseType]` attributes and Swagger documentation to all endpoints
- [x] 4.9 Run `dotnet build` to verify compilation
- [x] 4.10 Commit: `feat: add conversations controller with REST endpoints`

## 5. Unit Tests (Service Layer)

- [x] 5.1 Create `MessagingServiceTests` class in test project with mocked dependencies (IConversationRepository, IRepository<UserProfile>, IRepository<Message>, IRepository<MessageRead>, IUnitOfWork)
- [x] 5.2 Test CreateConversationAsync — valid creation, caller not in list, wrong participant count, non-existent profile, duplicate prevention, booking-linked always creates new
- [x] 5.3 Test SendMessageAsync — valid send, non-participant rejected, empty content rejected, non-existent conversation
- [x] 5.4 Test GetConversationsAsync — with conversations, empty result, pagination
- [x] 5.5 Test GetConversationAsync — participant access, non-participant rejected, non-existent conversation
- [x] 5.6 Test MarkAsReadAsync — first read, idempotent re-read, non-participant rejected, non-existent message
- [x] 5.7 Test GetUnreadCountAsync — with unread, all read, own messages excluded
- [x] 5.8 Run `dotnet test` to verify all unit tests pass
- [x] 5.9 Commit: `test: add messaging service unit tests`

## 6. Integration Tests (API Layer)

- [x] 6.1 Set up test fixture with WebApplicationFactory and in-memory/test database for messaging endpoints
- [x] 6.2 Test full conversation lifecycle: create → send messages → list → get → mark read → verify unread count
- [x] 6.3 Test authorization: non-participant cannot access conversation, send message, or mark read
- [x] 6.4 Test validation: empty content, invalid participant count, non-existent IDs
- [x] 6.5 Test duplicate conversation prevention returns existing conversation
- [x] 6.6 Test pagination: conversations list and messages within conversation
- [x] 6.7 Run `dotnet test` to verify all tests pass
- [x] 6.8 Commit: `test: add messaging API integration tests`

## 7. Verification

- [x] 7.1 Run `dotnet build` — solution compiles with no errors
- [x] 7.2 Run `dotnet test` — all tests pass
- [x] 7.3 Verify test coverage ≥ 80% for messaging service
- [x] 7.4 Manual Swagger test — verify all 6 endpoints appear and respond correctly
