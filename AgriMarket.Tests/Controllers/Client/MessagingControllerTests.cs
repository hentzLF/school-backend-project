using AgriMarket.BLL.Dtos;
using AgriMarket.BLL.Dtos.Messaging;
using AgriMarket.BLL.Dtos.Users;
using AgriMarket.BLL.Services;
using AgriMarket.Tests.Helpers;
using AgriMarket.Web.Areas.Client.Controllers;
using AgriMarket.Web.Areas.Client.ViewModels.Messaging;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace AgriMarket.Tests.Controllers.Client;

public class MessagingControllerTests
{
    private static readonly Guid UserId = Guid.NewGuid();
    private static readonly Guid ProfileId = Guid.NewGuid();
    private static readonly Guid OtherProfileId = Guid.NewGuid();
    private static readonly Guid ConversationId = Guid.NewGuid();

    private readonly Mock<IMessagingService> _messagingService = new();
    private readonly Mock<IUserService> _userService = new();

    private MessagingController CreateController()
    {
        _userService
            .Setup(s => s.GetProfileByUserIdAsync(UserId))
            .ReturnsAsync(new UserProfileDto { Id = ProfileId, FirstName = "Test", LastName = "User" });

        var controller = new MessagingController(_messagingService.Object, _userService.Object)
        {
            ControllerContext = ControllerContextFactory.WithAuthenticatedUser(UserId)
        };
        return controller;
    }

    [Fact]
    public async Task Index_CallsGetConversationsAsync_ReturnsView()
    {
        // Arrange
        var controller = CreateController();
        _messagingService
            .Setup(s => s.GetConversationsAsync(ProfileId, 1, It.IsAny<int>()))
            .ReturnsAsync(new PaginatedResponse<ConversationSummaryDto>
            {
                Items = new List<ConversationSummaryDto>
                {
                    new()
                    {
                        Id = ConversationId,
                        OtherParticipant = new ParticipantDto { ProfileId = OtherProfileId, FullName = "Other User" },
                        UnreadCount = 2,
                        CreatedAt = DateTime.UtcNow
                    }
                },
                Page = 1,
                PageSize = 20,
                TotalCount = 1
            });

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ConversationListViewModel>(viewResult.Model);
        Assert.Single(model.Conversations);
        Assert.Equal("Other User", model.Conversations[0].ParticipantName);
    }

    [Fact]
    public async Task Index_EmptyList_ReturnsViewWithEmptyModel()
    {
        // Arrange
        var controller = CreateController();
        _messagingService
            .Setup(s => s.GetConversationsAsync(ProfileId, 1, It.IsAny<int>()))
            .ReturnsAsync(new PaginatedResponse<ConversationSummaryDto>
            {
                Items = new List<ConversationSummaryDto>(),
                Page = 1,
                PageSize = 20,
                TotalCount = 0
            });

        // Act
        var result = await controller.Index();

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ConversationListViewModel>(viewResult.Model);
        Assert.Empty(model.Conversations);
    }

    [Fact]
    public async Task Details_CallsMarkAllAsReadAndGetConversation_ReturnsView()
    {
        // Arrange
        var controller = CreateController();
        _messagingService
            .Setup(s => s.MarkAllAsReadAsync(ProfileId, ConversationId))
            .ReturnsAsync(0);
        _messagingService
            .Setup(s => s.GetConversationAsync(ProfileId, ConversationId, 1, It.IsAny<int>()))
            .ReturnsAsync(new ConversationDto
            {
                Id = ConversationId,
                Participants = new List<ParticipantDto>
                {
                    new() { ProfileId = ProfileId, FullName = "Test User" },
                    new() { ProfileId = OtherProfileId, FullName = "Other User" }
                },
                Messages = new PaginatedResponse<MessageDto>
                {
                    Items = new List<MessageDto>(),
                    Page = 1,
                    PageSize = 20,
                    TotalCount = 0
                }
            });

        // Act
        var result = await controller.Details(ConversationId);

        // Assert
        var viewResult = Assert.IsType<ViewResult>(result);
        var model = Assert.IsType<ConversationDetailViewModel>(viewResult.Model);
        Assert.Equal(ConversationId, model.ConversationId);
        Assert.Equal("Other User", model.ParticipantName);
        _messagingService.Verify(s => s.MarkAllAsReadAsync(ProfileId, ConversationId), Times.Once);
    }

    [Fact]
    public async Task Details_ConversationNotFound_ReturnsNotFound()
    {
        // Arrange
        var controller = CreateController();
        _messagingService
            .Setup(s => s.MarkAllAsReadAsync(ProfileId, ConversationId))
            .ThrowsAsync(new KeyNotFoundException());

        // Act
        var result = await controller.Details(ConversationId);

        // Assert
        Assert.IsType<NotFoundResult>(result);
    }

    [Fact]
    public async Task SendMessage_ValidModel_CallsSendAndRedirects()
    {
        // Arrange
        var controller = CreateController();
        var model = new SendMessageViewModel
        {
            ConversationId = ConversationId,
            Content = "Hello!"
        };
        _messagingService
            .Setup(s => s.SendMessageAsync(ProfileId, ConversationId, It.IsAny<SendMessageDto>()))
            .ReturnsAsync(new MessageDto
            {
                Id = Guid.NewGuid(),
                ConversationId = ConversationId,
                SenderProfileId = ProfileId,
                SenderName = "Test User",
                Content = "Hello!",
                SentAt = DateTime.UtcNow
            });

        // Act
        var result = await controller.SendMessage(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        _messagingService.Verify(s => s.SendMessageAsync(ProfileId, ConversationId, It.IsAny<SendMessageDto>()), Times.Once);
    }

    [Fact]
    public async Task SendMessage_InvalidModel_RedirectsToDetails()
    {
        // Arrange
        var controller = CreateController();
        controller.ModelState.AddModelError("Content", "Required");
        var model = new SendMessageViewModel
        {
            ConversationId = ConversationId
        };

        // Act
        var result = await controller.SendMessage(model);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
    }

    [Fact]
    public async Task Create_CallsCreateConversationAndRedirects()
    {
        // Arrange
        var controller = CreateController();
        var bookingId = Guid.NewGuid();
        _messagingService
            .Setup(s => s.CreateConversationAsync(ProfileId, It.IsAny<CreateConversationDto>()))
            .ReturnsAsync((new ConversationDto
            {
                Id = ConversationId,
                BookingId = bookingId,
                Participants = new List<ParticipantDto>(),
                Messages = new PaginatedResponse<MessageDto>
                {
                    Items = new List<MessageDto>(),
                    Page = 1,
                    PageSize = 20,
                    TotalCount = 0
                }
            }, true));

        // Act
        var result = await controller.Create(OtherProfileId, bookingId);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        _messagingService.Verify(s => s.CreateConversationAsync(ProfileId, It.Is<CreateConversationDto>(
            d => d.ParticipantProfileIds.Contains(OtherProfileId) && d.BookingId == bookingId)), Times.Once);
    }

    [Fact]
    public async Task Create_WithoutBookingId_CallsCreateWithNullBookingId()
    {
        // Arrange
        var controller = CreateController();
        _messagingService
            .Setup(s => s.CreateConversationAsync(ProfileId, It.IsAny<CreateConversationDto>()))
            .ReturnsAsync((new ConversationDto
            {
                Id = ConversationId,
                Participants = new List<ParticipantDto>(),
                Messages = new PaginatedResponse<MessageDto>
                {
                    Items = new List<MessageDto>(),
                    Page = 1,
                    PageSize = 20,
                    TotalCount = 0
                }
            }, true));

        // Act
        var result = await controller.Create(OtherProfileId, null);

        // Assert
        var redirect = Assert.IsType<RedirectToActionResult>(result);
        Assert.Equal("Details", redirect.ActionName);
        _messagingService.Verify(s => s.CreateConversationAsync(ProfileId, It.Is<CreateConversationDto>(
            d => d.BookingId == null)), Times.Once);
    }
}
