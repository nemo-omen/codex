using System;
using Codex.Api.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Codex.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		builder.Entity<NoteLink>()
			.HasOne(nl => nl.Source)
			.WithMany(n => n.OutgoingLinks)
			.HasForeignKey(nl => nl.SourceId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Entity<NoteLink>()
			.HasOne(nl => nl.Target)
			.WithMany(n => n.IncomingLinks)
			.HasForeignKey(nl => nl.TargetId)
			.OnDelete(DeleteBehavior.Restrict);
	}

	public DbSet<Bookmark> Bookmarks { get; set; }
	public DbSet<Note> Notes { get; set; }
	public DbSet<NoteLink> NoteLinks { get; set; }
}
