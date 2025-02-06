using System;

namespace Codex.Api.Models;

public class NoteLink : BaseModel
{
	public required string Text { get; set; }
	public int StartIndex { get; set; }
	public int EndIndex { get; set; }
	public Guid SourceId { get; set; }
	public Note Source { get; set; } = null!;
	public Guid TargetId { get; set; }
	public Note Target { get; set; } = null!;
}
