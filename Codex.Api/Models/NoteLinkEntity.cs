using System;

namespace Codex.Api.Models;

public class NoteLinkEntity : BaseModel
{
	public required string Text { get; set; }
	public int StartIndex { get; set; }
	public int EndIndex { get; set; }
	public Guid SourceId { get; set; }
	public NoteEntity Source { get; set; }
	public Guid TargetId { get; set; }
	public NoteEntity Target { get; set; }
}
