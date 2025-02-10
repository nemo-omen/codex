using System;
using Codex.Api.Features.Notes;

namespace Codex.Tests.Unit.Notes;

public class NoteTests
{
	[Fact]
	public void Note_Constructor_ShouldInitializeWithGivenValues()
	{
		// Arrange
		var noteId = Guid.NewGuid();
		var userId = "test-user";
		var title = "Test Title";
		var content = "Test Content";
		var bookmarkId = Guid.NewGuid();
		var collectionId = Guid.NewGuid();

		// Act
		var note = new Note
		{
			Id = noteId,
			UserId = userId,
			Title = title,
			Content = content,
			BookmarkId = bookmarkId,
			CollectionId = collectionId
		};

		// Assert
		Assert.Equal(noteId, note.Id);
		Assert.Equal(userId, note.UserId);
		Assert.Equal(title, note.Title);
		Assert.Equal(content, note.Content);
		Assert.Equal(bookmarkId, note.BookmarkId);
		Assert.Equal(collectionId, note.CollectionId);
	}

	[Fact]
	public void Note_AddOutgoingLink_ShouldThrowExceptionForOverlappingLinks()
	{
		var note = new Note
		{
			UserId = Guid.NewGuid().ToString(),
			Id = Guid.NewGuid(),
			Title = "Test Note",
			Content = "Test Content",
			BookmarkId = Guid.NewGuid(),
			CollectionId = Guid.NewGuid(),
		};

		note.AddOutgoingLink(new NoteLink(Guid.NewGuid(), "Test", 0, 10, Guid.NewGuid(), Guid.NewGuid()));
		Assert.Throws<InvalidOperationException>(() =>
			note.AddOutgoingLink(new NoteLink(Guid.NewGuid(), "Test", 5, 15, Guid.NewGuid(), Guid.NewGuid())));
	}

	[Fact]
	public void Note_AddOutgoingLink_ShouldAddLink()
	{
		var note = new Note
		{
			UserId = Guid.NewGuid().ToString(),
			Id = Guid.NewGuid(),
			Title = "Test Note",
			Content = "Test Content",
			BookmarkId = Guid.NewGuid(),
			CollectionId = Guid.NewGuid(),
		};

		note.AddOutgoingLink(new NoteLink(Guid.NewGuid(), "Test", 0, 10, Guid.NewGuid(), Guid.NewGuid()));
		Assert.Single(note.OutgoingLinks);
	}

	[Fact]
	public void Note_AddOutgoingLink_ShouldAddLinkWhenNoOverlapExists()
	{
		var note = new Note
		{
			UserId = Guid.NewGuid().ToString(),
			Id = Guid.NewGuid(),
			Title = "Test Note",
			Content = "Test Content",
			BookmarkId = Guid.NewGuid(),
			CollectionId = Guid.NewGuid(),
		};

		var outgoingLink = new NoteLink(Guid.NewGuid(), "Test", 0, 10, Guid.NewGuid(), Guid.NewGuid());
		note.AddOutgoingLink(outgoingLink);
		var outgoingLink2 = new NoteLink(Guid.NewGuid(), "Test", 20, 30, Guid.NewGuid(), Guid.NewGuid());
		note.AddOutgoingLink(outgoingLink2);

		Assert.Contains(outgoingLink, note.OutgoingLinks);
		Assert.Contains(outgoingLink2, note.OutgoingLinks);
		Assert.Equal(2, note.OutgoingLinks.Count);
	}

	[Fact]
	public void Note_RemoveOutgoingLink_ShouldRemoveLinkWhenExists()
	{
		var note = new Note
		{
			UserId = Guid.NewGuid().ToString(),
			Id = Guid.NewGuid(),
			Title = "Test Note",
			Content = "Test Content",
			BookmarkId = Guid.NewGuid(),
			CollectionId = Guid.NewGuid(),
		};

		var outgoingLink = new NoteLink(Guid.NewGuid(), "Test", 0, 10, Guid.NewGuid(), Guid.NewGuid());

		note.AddOutgoingLink(outgoingLink);
		note.RemoveOutgoingLink(outgoingLink);
		Assert.Empty(note.OutgoingLinks);
	}

	[Fact]
	public void Note_AddIncomingLink_ShouldAddLink()
	{
		var note = new Note
		{
			UserId = Guid.NewGuid().ToString(),
			Id = Guid.NewGuid(),
			Title = "Test Note",
			Content = "Test Content",
			BookmarkId = Guid.NewGuid(),
			CollectionId = Guid.NewGuid(),
		};

		note.AddIncomingLink(new NoteLink(Guid.NewGuid(), "Test", 0, 10, Guid.NewGuid(), Guid.NewGuid()));
		Assert.Single(note.IncomingLinks);
	}

	[Fact]
	public void Note_RemoveIncomingLink_ShouldRemoveLink()
	{
		var note = new Note
		{
			UserId = Guid.NewGuid().ToString(),
			Id = Guid.NewGuid(),
			Title = "Test Note",
			Content = "Test Content",
			BookmarkId = Guid.NewGuid(),
			CollectionId = Guid.NewGuid(),
		};

		var incomingLink = new NoteLink(Guid.NewGuid(), "Test", 0, 10, Guid.NewGuid(), Guid.NewGuid());
		note.AddIncomingLink(incomingLink);
		note.RemoveIncomingLink(incomingLink);
		Assert.Empty(note.IncomingLinks);
	}

}
