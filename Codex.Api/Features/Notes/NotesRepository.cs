using System;
using Codex.Api.Data;
using Codex.Api.Features.Notes.Types;
using Codex.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Codex.Api.Features.Notes;

public interface INotesRepository
{
	Task<List<Note>> GetNotesAsync(string userId);
	Task<Guid> CreateNoteAsync(CreateNoteRequest request);
}

public class NotesRepository : INotesRepository
{
	private readonly ILogger<NotesRepository> _logger;
	private readonly ApplicationDbContext _context;

	public NotesRepository(ILogger<NotesRepository> logger, ApplicationDbContext context)
	{
		_logger = logger;
		_context = context;
	}

	public async Task<List<Note>> GetNotesAsync(string userId)
	{
		_logger.LogInformation($"Attempting to retrieve notes for user: {userId}");
		var notes = await _context.Notes
			.AsNoTracking()
			.Include(n => n.IncomingLinks)
			.Include(n => n.OutgoingLinks)
			.Where(n => n.UserId == userId)
			.ToListAsync();

		_logger.LogInformation($"Successfully retrieved {notes.Count} notes for user: {userId}");
		return notes;
	}

	public async Task<Guid> CreateNoteAsync(CreateNoteRequest request)
	{
		var note = new Note
		{
			Id = Guid.NewGuid(),
			Title = request.Title,
			Content = request.Content,
			UserId = request.UserId,
			BookmarkId = request.BookmarkId
		};

		await _context.Notes.AddAsync(note);
		await _context.SaveChangesAsync();
		return note.Id;
	}
}
