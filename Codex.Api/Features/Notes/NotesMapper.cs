using Codex.Api.Features.Notes.Types;
using Codex.Api.Models;

namespace Codex.Api.Features.Notes;

public static class NotesMapper
{
	public static Note FromEntity(NoteEntity entity)
	{
		var note = new Note
		{
			Id = entity.Id,
			Title = entity.Title,
			Content = entity.Content,
			UserId = entity.UserId,
		};

		if(entity.BookmarkId.HasValue) note.BookmarkId = entity.BookmarkId.Value;
		if(entity.CollectionId.HasValue) note.CollectionId = entity.CollectionId.Value;

		foreach (var link in entity.OutgoingLinks)
		{
			note.AddOutgoingLink(new NoteLink(link.Id, link.Text, link.StartIndex, link.EndIndex, link.SourceId, link.TargetId));
		}

		foreach (var link in entity.IncomingLinks)
		{
			note.AddIncomingLink(new NoteLink(link.Id, link.Text, link.StartIndex, link.EndIndex, link.SourceId, link.TargetId));
		}

		return note;
	}

	public static Note FromDto(NoteDto noteDto)
	{
		var note = new Note
		{
			Id = noteDto.Id,
			Title = noteDto.Title,
			Content = noteDto.Content,
			UserId = noteDto.UserId,
		};

		if (noteDto.BookmarkId is not null) note.BookmarkId = (Guid)noteDto.BookmarkId;

		if (noteDto.CollectionId is not null) note.CollectionId = (Guid)noteDto.CollectionId;

		return note;
	}

	public static NoteEntity ToEntity(Note note)
	{
		return new NoteEntity
		{
			Id = note.Id,
			Title = note.Title,
			Content = note.Content,
			UserId = note.UserId,
			BookmarkId = note.BookmarkId,
			CollectionId = note.CollectionId,
			OutgoingLinks = note.OutgoingLinks.Select(l => new NoteLinkEntity
			{
				Id = l.Id,
				Text = l.Text,
				StartIndex = l.StartIndex,
				EndIndex = l.EndIndex,
				SourceId = l.SourceId,
				TargetId = l.TargetId
			}).ToList(),
			IncomingLinks = note.IncomingLinks.Select(l => new NoteLinkEntity
			{
				Id = l.Id,
				Text = l.Text,
				StartIndex = l.StartIndex,
				EndIndex = l.EndIndex,
				SourceId = l.SourceId,
				TargetId = l.TargetId
			}).ToList()
		};
	}

	public static NoteEntity FromDtoToEntity(NoteDto noteDto)
	{
		var note = new NoteEntity
		{
			Id = noteDto.Id,
			Title = noteDto.Title,
			Content = noteDto.Content,
			UserId = noteDto.UserId,
		};

		if (noteDto.BookmarkId is not null)
		{
			note.BookmarkId = (Guid)noteDto.BookmarkId;
		}

		if (noteDto.CollectionId is not null)
		{
			note.CollectionId = (Guid)noteDto.CollectionId;
		}

		return note;
	}
}
