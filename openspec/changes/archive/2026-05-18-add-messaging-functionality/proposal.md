## Why

AgriMarket has messaging domain entities (`Conversation`, `ConversationParticipant`, `Message`, `MessageRead`) fully modeled and migrated, but no service layer, DTOs, repositories, or API endpoints exist. Users (providers and clients) currently have no way to communicate within the platform about bookings or services. Adding messaging functionality closes this gap and enables in-app communication tied to bookings.

## What Changes

- Add messaging DTOs in `AgriMarket.BLL/Dtos/Messaging/` for conversations, messages, and read receipts
- Add specialized repositories (`IConversationRepository`, `IMessageRepository`) in `AgriMarket.DAL` with complex queries (conversations with last message, paginated messages, unread counts)
- Add `IMessagingService` / `MessagingService` in `AgriMarket.BLL` handling conversation creation, message sending, read tracking, and authorization (only participants access their conversations)
- Add `ConversationsController` in `AgriMarket.Api` exposing REST endpoints for all messaging operations
- Add unit tests for `MessagingService` and integration tests for the controller
- Register new services and repositories in `BllServiceExtensions` and `DalServiceExtensions`

## Capabilities

### New Capabilities
- `messaging-api`: REST API endpoints for conversations and messages (create conversation, list conversations, get conversation, send message, mark read, unread count)
- `messaging-service`: Business logic for messaging — authorization, conversation lifecycle, message delivery, read tracking
- `messaging-repositories`: Data access for conversations and messages with optimized queries (last message per conversation, paginated messages, unread counts)

### Modified Capabilities

_(none — messaging is a new feature with no changes to existing specs)_

## Impact

- **Code**: New files across DAL (2 repository interfaces + implementations), BLL (1 service interface + implementation, ~6 DTOs), and API (1 controller). Registration additions in `BllServiceExtensions` and `DalServiceExtensions`.
- **APIs**: 6 new REST endpoints under `/api/v1/conversations` and `/api/v1/messages`.
- **Database**: No schema changes — all tables and indexes already exist from prior migrations.
- **Dependencies**: No new NuGet packages required.
- **Auth**: All endpoints require JWT authentication. Authorization enforced at service layer — only conversation participants can access conversations and messages.
