namespace AgriMarket.BLL.Dtos.Messaging;

public sealed class ParticipantDto
{
    public Guid ProfileId { get; init; }
    public string FullName { get; init; } = default!;
}
