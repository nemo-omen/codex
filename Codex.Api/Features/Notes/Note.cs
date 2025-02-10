using System;

namespace Codex.Api.Features.Notes;

public class Note
{
	public Guid Id { get; set; }
	public required string UserId { get; set; }
	public string Title { get; set; } = "Untitled";
	public string? Content { get; set; }
	public Guid BookmarkId { get; set; }
	public Guid? CollectionId { get; set; }
	private readonly HashSet<NoteLink> _outgoingLinks = [];
	private readonly HashSet<NoteLink> _incomingLinks = [];

	public IReadOnlyCollection<NoteLink> OutgoingLinks => _outgoingLinks;
	public IReadOnlyCollection<NoteLink> IncomingLinks => _incomingLinks;

	public void AddOutgoingLink(NoteLink link)
	{
		if (_outgoingLinks.Any(l => l.Overlaps(link)))
		{
			throw new InvalidOperationException("Cannot add overlapping links.");
		}

		_outgoingLinks.Add(link);
	}

	public void AddIncomingLink(NoteLink link)
	{
		_incomingLinks.Add(link);
	}

	public void RemoveOutgoingLink(NoteLink link)
	{
		_outgoingLinks.Remove(link);
	}

	public void RemoveIncomingLink(NoteLink link)
	{
		_incomingLinks.Remove(link);
	}
}
