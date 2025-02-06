using System.Security.Claims;
using Codex.Api.Exceptions;
using Codex.Api.Features.Bookmarks.Types;
using Codex.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Codex.Api.Features.Bookmarks;

[Route("api/[controller]")]
[ApiController]
public class BookmarksController : ControllerBase
{
	private readonly ILogger<BookmarksController> _logger;
	private readonly IBookmarksService _bookmarksService;

	public BookmarksController(ILogger<BookmarksController> logger, IBookmarksService bookmarksService)
	{
		_logger = logger;
		_bookmarksService = bookmarksService;
	}

	[HttpGet]
	[ProducesResponseType<List<BookmarkResponse>>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetAllBookmarks()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		try
		{
			var bookmarks = await _bookmarksService.GetBookmarksAsync(userId);
			return Ok(bookmarks.Select(b => new BookmarkResponse
			{
				Id = b.Id,
				Title = b.Title,
				Description = b.Description,
				Url = b.Url,
				UserId = b.UserId
			}));
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			return StatusCode(500, e.Message);
		}
	}

	/// <summary>
	/// Retrieves a bookmark by its ID.
	/// </summary>
	/// <param name="id">The ID of the bookmark to retrieve.</param>
	/// <returns>
	/// A <see cref="IActionResult"/> indicating the result of the action.
	/// </returns>
	/// <response code="200">The bookmark was retrieved successfully.</response>
	/// <response code="401">The request is unauthorized.</response>
	/// <response code="404">The bookmark with the given ID was not found.</response>
	/// <response code="500">An error occurred while retrieving the bookmark.</response>
	[HttpGet("{id:guid}")]
	[ProducesResponseType<BookmarkResponse>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetBookmarkByIdAsync(Guid id)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		try
		{
			var bookmark = await _bookmarksService.GetBookmarkByIdAsync(id);
			if (bookmark == null) return NotFound();
			return Ok(bookmark);
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			return StatusCode(500, e.Message);
		}
	}

	/// <summary>
	/// Creates a new bookmark with the given data.
	/// </summary>
	/// <param name="request">The request containing the data to create the bookmark with.</param>
	/// <returns>A <see cref="IActionResult"/> indicating the result of the action.</returns>
	/// <response code="201">The bookmark was created successfully.</response>
	/// <response code="422">The request was invalid.</response>
	[HttpPost]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
	public async Task<IActionResult> CreateBookmarkAsync([FromBody] CreateBookmarkRequest request)
	{
		if (!ModelState.IsValid)
		{
			return UnprocessableEntity(ModelState);
		}

		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		request.UserId = userId;

		try
		{
			var createBookmarkResult = await _bookmarksService.CreateBookmarkAsync(request);
			return Created();
		}
		catch (Exception e)
		{
			return StatusCode(500, e.Message);
		}
	}

	[HttpPut]
	[Authorize]
	[ProducesResponseType(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> UpdateBookmarkAsync([FromBody] UpdateBookmarkRequest request)
	{
		if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		try
		{
			await _bookmarksService.UpdateBookmarkAsync(request);
			return Ok();
		}
		catch (NotFoundException e)
		{
			return NotFound(e.Message);
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
	public async Task<IActionResult> DeleteBookmarkAsync(Guid id)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		try
		{
			await _bookmarksService.DeleteBookmarkAsync(id);
			return Ok();
		}
		catch (NotFoundException e)
		{
			return NotFound(e.Message);
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			return StatusCode(500, e.Message);
		}
	}
}

