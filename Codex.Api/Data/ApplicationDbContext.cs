using System;
using Codex.Api.Models;
using EntityFramework.Exceptions.PostgreSQL;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Codex.Api.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
	public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
		: base(options)
	{
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		optionsBuilder.UseExceptionProcessor();
		base.OnConfiguring(optionsBuilder);
	}

	protected override void OnModelCreating(ModelBuilder builder)
	{
		base.OnModelCreating(builder);

		// Bookmark should be unique for URL and user
		builder.Entity<BookmarkEntity>()
			.HasIndex(b => new { b.Url, b.UserId })
			.IsUnique();

		// Note should be unique for title, user, and bookmark
		builder.Entity<NoteEntity>()
			.HasIndex(n => new { n.Title, n.UserId, n.BookmarkId })
			.IsUnique();

		// NoteLink should be unique for start and end index, source, and target
		builder.Entity<NoteLinkEntity>()
			.HasIndex(nl => new { nl.StartIndex, nl.EndIndex, nl.SourceId, nl.TargetId })
			.IsUnique();

		// Collection should be unique for name and user
		builder.Entity<CollectionEntity>()
			.HasIndex(c => new { c.Name, c.UserId })
			.IsUnique();

		builder.Entity<NoteLinkEntity>()
			.HasOne(nl => nl.Source)
			.WithMany(n => n.OutgoingLinks)
			.HasForeignKey(nl => nl.SourceId)
			.OnDelete(DeleteBehavior.Restrict);

		builder.Entity<NoteLinkEntity>()
			.HasOne(nl => nl.Target)
			.WithMany(n => n.IncomingLinks)
			.HasForeignKey(nl => nl.TargetId)
			.OnDelete(DeleteBehavior.Restrict);
	}

	public DbSet<BookmarkEntity> Bookmarks { get; set; }
	public DbSet<NoteEntity> Notes { get; set; }
	public DbSet<NoteLinkEntity> NoteLinks { get; set; }
	public DbSet<CollectionEntity> Collections { get; set; }
}
