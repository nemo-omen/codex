using System;
using Codex.Api.Features.Notes.Types;
using Codex.Api.Models;

namespace Codex.Api.Features.Notes;

public interface INotesService
{
	Task<Guid> CreateNoteAsync(CreateNoteRequest request);
	Task<Note> GetNoteByIdAsync(Guid id);
	Task<List<Note>> GetNotesAsync(string userId);
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

	public async Task<List<Note>> GetNotesAsync(string userId)
	{
		return await _notesRepository.GetNotesAsync(userId);
	}
}
