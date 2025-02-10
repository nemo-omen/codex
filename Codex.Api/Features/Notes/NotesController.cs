using System;
using System.Security.Claims;
using Codex.Api.Features.Notes.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codex.Api.Features.Notes;

[ApiController]
[Route("api/[controller]")]
public class NotesController : ControllerBase
{
	private readonly ILogger<NotesController> _logger;
	private readonly INotesService _noteService;

	public NotesController(ILogger<NotesController> logger, INotesService noteService)
	{
		_logger = logger;
		_noteService = noteService;
	}

	[HttpGet]
	[Authorize]
	[ProducesResponseType<List<NoteResponse>>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetNotesAsync()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		try
		{
			var notes = await _noteService.GetNotesAsync(userId);
			return Ok(notes);
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			return StatusCode(500, e.Message);
		}
	}

	[HttpGet("{id:guid}")]
	[Authorize]
	[ProducesResponseType<NoteResponse>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetNoteByIdAsync(Guid id)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		try
		{
			var note = await _noteService.GetNoteByIdAsync(id);
			return Ok(note);
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			return StatusCode(500, e.Message);
		}
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

	[HttpPut]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UpdateNoteAsync([FromBody] EditNoteRequest request)
	{
		if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		try
		{
			await _noteService.UpdateNoteAsync(request);
			return Ok();
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			return StatusCode(500, e.Message);
		}
	}

	[HttpDelete("{id:guid}")]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> DeleteNoteAsync(Guid id)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		try
		{
			await _noteService.DeleteNoteAsync(id);
			return Ok();
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			return StatusCode(500, e.Message);
		}
	}
}
