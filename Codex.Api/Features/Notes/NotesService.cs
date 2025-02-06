using System;
using Codex.Api.Features.Notes.Types;

namespace Codex.Api.Features.Notes;

public interface INotesService
{
	Task<Guid> CreateNoteAsync(CreateNoteRequest request);
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
}
