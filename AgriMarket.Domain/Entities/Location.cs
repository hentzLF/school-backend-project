using System.ComponentModel.DataAnnotations;

namespace AgriMarket.Domain.Entities;

public class Location
{
    public Guid Id { get; set; }

    [Range(-90, 90)]
    public double Latitude { get; set; }

    [Range(-180, 180)]
    public double Longitude { get; set; }

    public string Address { get; set; } = default!;

    public string City { get; set; } = default!;

    public string? PostalCode { get; set; }

    public string Country { get; set; } = default!;
}