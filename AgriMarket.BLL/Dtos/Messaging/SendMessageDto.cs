using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Messaging;

public sealed class SendMessageDto
{
    [Required]
    public string Content { get; init; } = default!;
}
