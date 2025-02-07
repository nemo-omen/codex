using System;
using Codex.Api.Features.Notes.Types;
using Codex.Api.Models;

namespace Codex.Api.Features.Notes;

public interface INotesService
{
	Task<Guid> CreateNoteAsync(CreateNoteRequest request);
	Task<Note> GetNoteByIdAsync(Guid id);
	Task<List<NoteResponse>> GetNotesAsync(string userId);
	Task UpdateNoteAsync(EditNoteRequest request);
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
		return await _notesRepository.CreateNoteAsync(request);
	}

	public async Task<Note> GetNoteByIdAsync(Guid id)
	{
		return await _notesRepository.GetNoteByIdAsync(id);
	}

	public async Task<List<NoteResponse>> GetNotesAsync(string userId)
	{
		var notes = await _notesRepository.GetNotesAsync(userId);
		var responses = notes.Select(n => new NoteResponse
		{
			Id = n.Id,
			Title = n.Title,
			Content = n.Content,
			UserId = n.UserId,
			BookmarkId = n.BookmarkId,
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
		}).ToList();
		return responses;
	}

	public Task UpdateNoteAsync(EditNoteRequest request)
	{
		return _notesRepository.UpdateNoteAsync(request);
	}
}
