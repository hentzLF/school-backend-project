using System.ComponentModel.DataAnnotations;
using AgriMarket.Domain.Enums;

namespace AgriMarket.Web.Areas.Client.ViewModels;

public class RegisterViewModel
{
    [Required]
    [EmailAddress]
    public string Email { get; set; } = default!;

    [Required]
    [DataType(DataType.Password)]
    [MinLength(6)]
    public string Password { get; set; } = default!;

    [Required]
    public string FirstName { get; set; } = default!;

    [Required]
    public string LastName { get; set; } = default!;

    [Required]
    public RoleType Role { get; set; } = RoleType.Farmer;
}
