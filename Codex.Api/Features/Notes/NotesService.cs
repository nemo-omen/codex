using System;
using Codex.Api.Exceptions;
using Codex.Api.Features.Notes.Types;
using Codex.Api.Models;

namespace Codex.Api.Features.Notes;

public interface INotesService
{
	Task<Guid> CreateNoteAsync(CreateNoteRequest request);
	Task<NoteEntity> GetNoteByIdAsync(Guid id);
	Task<List<NoteResponse>> GetNotesAsync(string userId);
	Task UpdateNoteAsync(EditNoteRequest request);
	Task DeleteNoteAsync(Guid id);
}

public class NotesService : INotesService
{
	private readonly ILogger<NotesService> _logger;
	private readonly INotesRepository _notesRepository;

	public NotesService(ILogger<NotesService> logger, INotesRepository notesRepository)
	{
		_logger = logger;
		_notesRepository = notesRepository;
	}

	public async Task<Guid> CreateNoteAsync(CreateNoteRequest request)
	{
		var note = new NoteEntity
		{
			Id = Guid.NewGuid(),
			Title = request.Title,
			Content = request.Content,
			UserId = request.UserId,
			BookmarkId = request.BookmarkId
		};

		return await _notesRepository.CreateNoteAsync(note);
	}

	public async Task<NoteEntity> GetNoteByIdAsync(Guid id)
	{
		return await _notesRepository.GetNoteByIdAsync(id);
	}

	public async Task<List<NoteResponse>> GetNotesAsync(string userId)
	{
		var notes = await _notesRepository.GetNotesAsync(userId);
		var responses = notes.Select(n =>
		{
			var response =  new NoteResponse
			{
				Id = n.Id,
				Title = n.Title,
				Content = n.Content,
				UserId = n.UserId,
				IncomingLinks = n.IncomingLinks.Select(l => new NoteLinkResponse
				{
					Text = l.Text,
					StartIndex = l.StartIndex,
					EndIndex = l.EndIndex,
					SourceId = l.SourceId,
					TargetId = l.TargetId
				}).ToList(),
				OutgoingLinks = n.OutgoingLinks.Select(l => new NoteLinkResponse
				{
					Text = l.Text,
					StartIndex = l.StartIndex,
					EndIndex = l.EndIndex,
					SourceId = l.SourceId,
					TargetId = l.TargetId
				}).ToList()
			};
			if(n.BookmarkId.HasValue) response.BookmarkId = n.BookmarkId.Value;
			if(n.CollectionId.HasValue) response.CollectionId = n.CollectionId.Value;
			return response;
		}).ToList();
		return responses;
	}

	public async Task UpdateNoteAsync(EditNoteRequest request)
	{
		var entity = await _notesRepository.GetNoteByIdAsync(request.Id);
		if (entity == null) throw new NotFoundException("Note not found.");

		var note = NotesMapper.FromEntity(entity);
		note.Title = request.Title ?? note.Title;
		note.Content = request.Content ?? note.Content;
		note.UserId = request.UserId;

		if (request.BookmarkId is not null) note.BookmarkId = (Guid)request.BookmarkId;

		if (request.IncomingLinks is not null)
		{
			foreach (var link in request.IncomingLinks)
			{
				if (link.Id is null)
				{
					note.AddIncomingLink(new NoteLink(link.Text, link.StartIndex, link.EndIndex, link.SourceId, link.TargetId));
				}
				else
				{
					note.AddIncomingLink(new NoteLink((Guid)link.Id, link.Text, link.StartIndex, link.EndIndex, link.SourceId, link.TargetId));
				}
			}
		}

		if (request.OutgoingLinks is not null)
		{
			foreach (var link in request.OutgoingLinks)
			{
				if (link.Id is null)
				{
					note.AddOutgoingLink(new NoteLink(link.Text, link.StartIndex, link.EndIndex, link.SourceId, link.TargetId));
				}
				else
				{
					note.AddOutgoingLink(new NoteLink((Guid)link.Id, link.Text, link.StartIndex, link.EndIndex, link.SourceId, link.TargetId));
				}
			}
		}
		await _notesRepository.UpdateNoteAsync(NotesMapper.ToEntity(note));
	}

	public async Task DeleteNoteAsync(Guid id)
	{
		var note = await _notesRepository.GetNoteByIdAsync(id);
		if (note is null) return;

		foreach (var link in note.IncomingLinks)
		{
			link.Source.OutgoingLinks.Remove(link);
			await _notesRepository.UpdateNoteAsync(link.Source);
		}

		foreach (var link in note.OutgoingLinks)
		{
			link.Target.IncomingLinks.Remove(link);
			await _notesRepository.UpdateNoteAsync(link.Target);
		}

		await _notesRepository.DeleteNoteAsync(id);
	}
}
