using System;
using Codex.Api.Data;
using Codex.Api.Exceptions;
using Codex.Api.Features.Notes.Types;
using Codex.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Codex.Api.Features.Notes;

public interface INotesRepository
{
	Task<List<NoteEntity>> GetNotesAsync(string userId);
	Task<NoteEntity> GetNoteByIdAsync(Guid id);
	Task<Guid> CreateNoteAsync(NoteEntity request);
	Task<NoteEntity> UpdateNoteAsync(NoteEntity request);
	Task DeleteNoteAsync(Guid id);
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

	public async Task<List<NoteEntity>> GetNotesAsync(string userId)
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

	public async Task<NoteEntity> GetNoteByIdAsync(Guid id)
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

	public async Task<Guid> CreateNoteAsync(NoteEntity note)
	{
		await _context.Notes.AddAsync(note);
		await _context.SaveChangesAsync();
		return note.Id;
	}

	public async Task<NoteEntity> UpdateNoteAsync(NoteEntity note)
	{
		var existingNote = await _context.Notes
			.Include(n => n.IncomingLinks)
			.Include(n => n.OutgoingLinks)
				.ThenInclude(l => l.Target)
			.FirstOrDefaultAsync(n => n.Id == note.Id);

		if (existingNote == null)
			throw new NotFoundException("Note not found.");

		existingNote.Title = note.Title ?? existingNote.Title;
		existingNote.Content = note.Content ?? existingNote.Content;

		if (note.OutgoingLinks != null)
		{
			foreach (var link in note.OutgoingLinks)
			{
				var existingLink = existingNote.OutgoingLinks
					.FirstOrDefault(l => l.TargetId == link.TargetId);

				var targetNote = await _context.Notes
					.FirstOrDefaultAsync(n => n.Id == link.TargetId);

				if (targetNote is null) throw new NotFoundException("Target note not found.");

				if (existingLink is null)
				{
					var newLink = new NoteLinkEntity
					{
						Id = Guid.NewGuid(),
						Text = link.Text!,
						SourceId = existingNote.Id,
						TargetId = link.TargetId,
						StartIndex = link.StartIndex,
						EndIndex = link.EndIndex,
					};

					existingNote.OutgoingLinks.Add(newLink);
					targetNote.IncomingLinks.Add(newLink);
					await _context.NoteLinks.AddAsync(newLink);
				}
				else
				{
					// check whether the link target has changed
					var originalLinkTarget = existingLink.Target;
					if (originalLinkTarget is not null)
					{
						// if the target has changed, remove the link
						if (originalLinkTarget.Id != link.TargetId)
						{
							originalLinkTarget.OutgoingLinks.Remove(existingLink);
						}
					}

					// update the existing link and add it to the target
					existingLink.TargetId = link.TargetId;
					existingLink.Text = link.Text ?? existingLink.Text;
					existingLink.SourceId = link.SourceId;
					targetNote.IncomingLinks.Add(existingLink);
				}
				// _context.Update(targetNote);
			}
		}

		if (note.IncomingLinks != null)
		{
			foreach (var link in note.IncomingLinks)
			{
				var existingLink = existingNote.IncomingLinks.FirstOrDefault(l => l.TargetId == link.TargetId);
				if (existingLink == null)
				{
					var newLink = new NoteLinkEntity
					{
						Text = link.Text!,
						SourceId = existingNote.Id,
						TargetId = link.TargetId,
						StartIndex = link.StartIndex,
						EndIndex = link.EndIndex
					};

					existingNote.IncomingLinks.Add(newLink);
					await _context.NoteLinks.AddAsync(newLink);
				}
				else
				{
					existingLink.TargetId = link.TargetId;
					existingLink.Text = link.Text ?? existingLink.Text;
					existingLink.SourceId = link.SourceId;
				}
			}
		}

		// _context.Update(note);
		await _context.SaveChangesAsync();
		return existingNote;
	}

	public async Task DeleteNoteAsync(Guid id)
	{
		var note = await _context.Notes
			.Include(n => n.IncomingLinks)
				.ThenInclude(l => l.Source)
			.Include(n => n.OutgoingLinks)
				.ThenInclude(l => l.Target)
			.FirstOrDefaultAsync(n => n.Id == id) ??
				throw new NotFoundException("Note not found.");

		foreach (var link in note.IncomingLinks)
		{
			link.Source.OutgoingLinks.Remove(link);
			_context.NoteLinks.Remove(link);
		}

		foreach (var link in note.OutgoingLinks)
		{
			link.Target.IncomingLinks.Remove(link);
			_context.NoteLinks.Remove(link);
		}

		_context.Notes.Remove(note);
		await _context.SaveChangesAsync();
	}
}
