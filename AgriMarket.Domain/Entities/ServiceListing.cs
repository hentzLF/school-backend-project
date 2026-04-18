using System.Security.Cryptography.X509Certificates;
using AgriMarket.Domain.Enums;

namespace AgriMarket.Domain.Entities;

public class ServiceListing
{
    public Guid Id {get; set;}

    public string Title {get; set;} = default!;
    public string? Description {get; set;}
    public decimal PricePerHectare {get; set;}
    public bool IsActive {get; set;}

    // Foreign Keys
    public Guid UserProfileId {get; set;}
    public Guid ServiceCategoryId {get; set;}

    public Guid? LucationId {get; set;}
    
    // Navigation
    public UserProfile? UserProfile {get; set;}
    public ServiceCategory? ServiceCategory {get; set;}
    public Location? Location {get; set;}
    public ICollection<Equipment>? Equipments {get; set;}
    public ICollection<Availability>? Availabilities { get; set; }
}