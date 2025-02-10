using System;
using Codex.Api.Exceptions;
using Codex.Api.Features.Bookmarks.Mappers;
using Codex.Api.Features.Bookmarks.Types;
using Codex.Api.Models;
using EntityFramework.Exceptions.Common;
using Microsoft.Build.Framework;

namespace Codex.Api.Features.Bookmarks;

public interface IBookmarksService
{
	Task<BookmarkEntity?> GetBookmarkByIdAsync(Guid id);
	Task<List<BookmarkEntity>> GetBookmarksAsync(string userId);
	Task<Guid> CreateBookmarkAsync(CreateBookmarkRequest request);
	Task UpdateBookmarkAsync(UpdateBookmarkRequest bookmark);
	Task DeleteBookmarkAsync(Guid id);
}

public class BookmarksService : IBookmarksService
{
	private readonly ILogger<BookmarksService> _logger;
	private readonly IBookmarksRepository _bookmarksRepository;

	public BookmarksService(ILogger<BookmarksService> logger, IBookmarksRepository bookmarksRepository)
	{
		_logger = logger;
		_bookmarksRepository = bookmarksRepository;
	}

	public async Task<BookmarkEntity?> GetBookmarkByIdAsync(Guid id)
	{
		return await _bookmarksRepository.GetBookmarkByIdAsync(id);
	}

	public async Task<List<BookmarkEntity>> GetBookmarksAsync(string userId)
	{
		return await _bookmarksRepository.GetBookmarksAsync(userId);
	}

	public async Task<Guid> CreateBookmarkAsync(CreateBookmarkRequest request)
	{
		if (string.IsNullOrWhiteSpace(request.UserId)) throw new ArgumentException("UserId is required.");

		var bookmark = new BookmarkEntity
		{
			Id = Guid.NewGuid(),
			Title = request.Title,
			Url = request.Url,
			Description = request.Description,
			UserId = request.UserId,
		};

		return await _bookmarksRepository.CreateBookmarkAsync(bookmark);
	}

	public async Task UpdateBookmarkAsync(UpdateBookmarkRequest request)
	{
		var bookmark = BookmarkMapper.FromUpdateBookmarkRequest(request);
		try
		{
			var result = await _bookmarksRepository.UpdateBookmarkAsync(bookmark);
			if (!result) throw new NotFoundException("Bookmark not found.");
		}
		catch (UniqueConstraintException e)
		{
			_logger.LogError(e.Message);
			throw;
		}
	}

	public async Task DeleteBookmarkAsync(Guid id)
	{
		await _bookmarksRepository.DeleteBookmarkAsync(id);
	}
}
