using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace TheAggregate.Api.Models;

public class ApplicationUser : IdentityUser
{
    [MaxLength(255)]
    public string? Name { get; set; }
}