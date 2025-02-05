using System.Security.Claims;
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
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult Get()
    {
        return Ok();
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
}

