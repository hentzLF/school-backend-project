namespace AgriMarket.Domain.Entities;

public class ServiceCategory
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string? Description { get; set; }
}