using System;
using System.Collections.ObjectModel;
using System.Security.Claims;
using Codex.Api.Exceptions;
using Codex.Api.Features.Collections.Types;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Codex.Api.Features.Collections;

[ApiController]
[Route("api/[controller]")]
public class CollectionsController : ControllerBase
{
	private readonly ILogger<CollectionsController> _logger;
	private readonly ICollectionsService _collectionsService;
	public CollectionsController(ILogger<CollectionsController> logger, ICollectionsService collectionsService)
	{
		_logger = logger;
		_collectionsService = collectionsService;
	}

	[HttpGet]
	[Authorize]
	[ProducesResponseType<List<CollectionResponse>>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetCollectionsAsync()
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		return Ok(await _collectionsService.GetCollectionsAsync(userId));
	}

	[HttpGet("{id:guid}")]
	[Authorize]
	[ProducesResponseType<CollectionResponse>(StatusCodes.Status200OK)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status404NotFound)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> GetCollectionByIdAsync(Guid id)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		try
		{
			var collection = await _collectionsService.GetCollectionByIdAsync(id);
			return Ok(collection);
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

	[HttpPost]
	[Authorize]
	[ProducesResponseType<CollectionResponse>(StatusCodes.Status201Created)]
	[ProducesResponseType(StatusCodes.Status401Unauthorized)]
	[ProducesResponseType(StatusCodes.Status422UnprocessableEntity)]
	[ProducesResponseType(StatusCodes.Status409Conflict)]
	[ProducesResponseType(StatusCodes.Status500InternalServerError)]
	public async Task<IActionResult> CreateCollectionAsync([FromBody] CreateCollectionRequest request)
	{
		if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		request.UserId = userId;

		try
		{
			var result = await _collectionsService.CreateCollectionAsync(request);
			return Ok(new { id = result });
		}
		catch (ConflictException e)
		{
			return Conflict(e.Message);
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
	public async Task<IActionResult> UpdateCollectionAsync([FromBody] UpdateCollectionRequest request)
	{
		if (!ModelState.IsValid) return UnprocessableEntity(ModelState);

		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		request.UserId = userId;
		try
		{
			await _collectionsService.UpdateCollectionAsync(request);
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
	public async Task<IActionResult> DeleteCollectionAsync(Guid id)
	{
		var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);

		if (string.IsNullOrWhiteSpace(userId)) return Unauthorized();

		try
		{
			await _collectionsService.DeleteCollectionAsync(id);
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
