using System.ComponentModel.DataAnnotations;

namespace AgriMarket.BLL.Dtos.Messaging;

public sealed class CreateConversationDto
{
    [Required]
    public IList<Guid> ParticipantProfileIds { get; init; } = [];

    public Guid? BookingId { get; init; }
}
