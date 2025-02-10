using System;

namespace Codex.Api.Features.Notes;

public class NoteLink
{
	public Guid Id { get; set; }
	public string Text { get; set; }
	public int StartIndex { get; set; }
	public int EndIndex { get; set; }
	public Guid SourceId { get; set; }
	public Guid TargetId { get; set; }

	public NoteLink() { }

	public NoteLink(string text, int startIndex, int endIndex, Guid sourceId, Guid targetId)
	{
		Id = Guid.NewGuid();
		Text = text;
		StartIndex = startIndex;
		EndIndex = endIndex;
		SourceId = sourceId;
		TargetId = targetId;
	}

	public NoteLink(Guid id, string text, int startIndex, int endIndex, Guid sourceId, Guid targetId)
	{
		Id = id;
		Text = text;
		StartIndex = startIndex;
		EndIndex = endIndex;
		SourceId = sourceId;
		TargetId = targetId;
	}

	public bool Overlaps(NoteLink other)
	{
		return StartIndex < other.EndIndex && EndIndex > other.StartIndex;
	}
}
