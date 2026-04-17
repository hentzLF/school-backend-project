namespace AgriMarket.Domain.Entities;

public class Location
{
    public Guid Id { get; set; }

    public double Latitude { get; set; }

    public double Longitude { get; set; }

    public string Address { get; set; } = default!;

    public string City { get; set; } = default!;

    public string? PostalCode { get; set; }

    public string Country { get; set; } = default!;
}