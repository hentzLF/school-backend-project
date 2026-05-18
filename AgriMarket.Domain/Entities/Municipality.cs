namespace AgriMarket.Domain.Entities;

public class Municipality
{
    public Guid Id { get; set; }

    public string Name { get; set; } = default!;

    public string EhakCode { get; set; } = default!;

    public Guid CountyId { get; set; }

    public County? County { get; set; }

    public ICollection<Location>? Locations { get; set; }
}
