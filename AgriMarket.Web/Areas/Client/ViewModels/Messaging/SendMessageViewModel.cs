using System.ComponentModel.DataAnnotations;

namespace AgriMarket.Web.Areas.Client.ViewModels.Messaging;

public class SendMessageViewModel
{
    public Guid ConversationId { get; set; }

    [Required]
    public string Content { get; set; } = default!;
}
