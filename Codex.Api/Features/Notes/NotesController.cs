using System;
using System.Security.Claims;
using Codex.Api.Features.Notes.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codex.Api.Features.Notes;

[ApiController]
[Route("api/[controller]")]
public class NotesController : Controller
{
	private readonly ILogger<NotesController> _logger;
	private readonly INotesService _noteService;

	public NotesController(ILogger<NotesController> logger, INotesService noteService)
	{
		_logger = logger;
		_noteService = noteService;
	}

	[HttpPost]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> CreateNoteAsync([FromBody] CreateNoteRequest request)
	{
		if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		try
		{
			var createResult = await _noteService.CreateNoteAsync(request);
			return Ok(createResult);
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			return StatusCode(500, e.Message);
		}
	}
}
