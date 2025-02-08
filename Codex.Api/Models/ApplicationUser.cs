using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Identity;

namespace Codex.Api.Models;

public class ApplicationUser : IdentityUser
{
	[MaxLength(255)]
	public string? Name { get; set; }
	public ICollection<Collection>? Collections { get; set; } = [];
}