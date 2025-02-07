using System;
using Codex.Api.Data;
using Codex.Api.Exceptions;
using Codex.Api.Features.Notes.Types;
using Codex.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Codex.Api.Features.Notes;

public interface INotesRepository
{
	Task<List<Note>> GetNotesAsync(string userId);
	Task<Note> GetNoteByIdAsync(Guid id);
	Task<Guid> CreateNoteAsync(CreateNoteRequest request);
	Task<Note> UpdateNoteAsync(EditNoteRequest request);
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

	public async Task<Note> GetNoteByIdAsync(Guid id)
	{
		var note = await _context.Notes
			.AsNoTracking()
			.Include(n => n.IncomingLinks)
			.Include(n => n.OutgoingLinks)
			.FirstOrDefaultAsync(n => n.Id == id);

		if (note == null)
			throw new NotFoundException("Note not found.");

		return note;
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

	public async Task<Note> UpdateNoteAsync(EditNoteRequest request)
	{
		var note = await _context.Notes
			.Include(n => n.IncomingLinks)
			.Include(n => n.OutgoingLinks)
			.FirstOrDefaultAsync(n => n.Id == request.Id);

		if (note == null)
			throw new NotFoundException("Note not found.");

		note.Title = request.Title ?? note.Title;
		note.Content = request.Content ?? note.Content;

		if (request.OutgoingLinks != null)
		{
			foreach (var link in request.OutgoingLinks)
			{
				var existingLink = note.OutgoingLinks.FirstOrDefault(l => l.TargetId == link.TargetId);
				if (existingLink == null)
				{
					note.OutgoingLinks.Add(new NoteLink
					{
						Text = link.Text,
						Source = note,
						TargetId = link.TargetId
					});
				}
				else
				{
					existingLink.TargetId = link.TargetId;
					existingLink.Text = link.Text ?? existingLink.Text;
					existingLink.SourceId = link.SourceId;
				}
			}
		}

		if (request.IncomingLinks != null)
		{
			foreach (var link in request.IncomingLinks)
			{
				var existingLink = note.IncomingLinks.FirstOrDefault(l => l.TargetId == link.TargetId);
				if (existingLink == null)
				{
					note.IncomingLinks.Add(new NoteLink
					{
						Text = link.Text,
						Source = note,
						TargetId = link.TargetId
					});
				}
				else
				{
					existingLink.TargetId = link.TargetId;
					existingLink.Text = link.Text ?? existingLink.Text;
					existingLink.SourceId = link.SourceId;
				}
			}
		}

		_context.Update(note);
		await _context.SaveChangesAsync();
		return note;
	}
}
