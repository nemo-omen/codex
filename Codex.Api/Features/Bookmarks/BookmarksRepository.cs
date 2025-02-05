using System;
using Codex.Api.Data;
using Codex.Api.Models;
using Microsoft.EntityFrameworkCore;

namespace Codex.Api.Features.Bookmarks;

public interface IBookmarksRepository
{
	Task<Bookmark?> GetBookmarkByIdAsync(Guid id);
	Task<Bookmark?> GetBookmarkByUrlAsync(string url);
	Task<Guid> CreateBookmarkAsync(Bookmark bookmark);
	Task UpdateBookmarkAsync(Bookmark bookmark);
}

public class BookmarksRepository : IBookmarksRepository
{
	private readonly ILogger<BookmarksRepository> _logger;
	private readonly ApplicationDbContext _context;

	public BookmarksRepository(ILogger<BookmarksRepository> logger, ApplicationDbContext context)
	{
		_logger = logger;
		_context = context;
	}

	/// <summary>
	/// Retrieves a bookmark from the database with the given ID, or null if it does not exist.
	/// </summary>
	/// <param name="id">The ID of the bookmark to retrieve.</param>
	/// <returns>The bookmark with the given ID if it exists, or null.</returns>
	public async Task<Bookmark?> GetBookmarkByIdAsync(Guid id)
	{
		_logger.LogInformation($"Attempting to retrieve bookmark with ID: {id}");
		var bookmark = await _context.Bookmarks
			.AsNoTracking()
			.FirstOrDefaultAsync(b => b.Id == id);

		if (bookmark == null)
		{
			_logger.LogWarning($"Bookmark with ID: {id} not found.");
		}
		else
		{
			_logger.LogInformation($"Bookmark with ID: {id} retrieved successfully.");
		}

		return bookmark;
	}

	/// <summary>
	/// Retrieves a bookmark from the database with the given URL, or null if it does not exist.
	/// </summary>
	/// <param name="url">The URL of the bookmark to retrieve.</param>
	/// <returns>The bookmark with the given URL if it exists, or null.</returns>
	public async Task<Bookmark?> GetBookmarkByUrlAsync(string url)
	{
		_logger.LogInformation($"Attempting to retrieve bookmark with URL: {url}");
		var bookmark = await _context.Bookmarks
			.AsNoTracking()
			.FirstOrDefaultAsync(b => b.Url == url);

		if (bookmark == null)
		{
			_logger.LogWarning($"Bookmark with URL: {url} not found.");
		}
		else
		{
			_logger.LogInformation($"Bookmark with URL: {url} retrieved successfully.");
		}

		return bookmark;
	}


	/// <summary>
	/// Creates a new bookmark in the database.
	/// </summary>
	/// <param name="bookmark">The data for the bookmark to be created.</param>
	/// <returns>The ID of the created bookmark.</returns>
	/// <remarks>If the bookmark with the given ID already exists, a warning is logged and the ID of the existing bookmark is returned.</remarks>
	public async Task<Guid> CreateBookmarkAsync(Bookmark bookmark)
	{
		_logger.LogInformation($"Attempting to create bookmark: {bookmark}");
		_context.Bookmarks.Add(bookmark);
		try
		{
			await _context.SaveChangesAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			throw;
		}
		_logger.LogInformation($"Bookmark created successfully with ID: {bookmark.Id}");

		return bookmark.Id;
	}

	/// <summary>
	/// Updates an existing bookmark in the database with the provided data.
	/// </summary>
	/// <param name="bookmark">The bookmark data to update, including the ID of the bookmark to be updated.</param>
	/// <exception cref="ArgumentException">Thrown when the ID of the bookmark is not provided.</exception>
	/// <remarks>If the bookmark with the given ID does not exist, a warning is logged and no update is performed.</remarks>
	public async Task UpdateBookmarkAsync(Bookmark bookmark)
	{
		if (bookmark.Id == Guid.Empty)
			throw new ArgumentException("Id is required.");

		_logger.LogInformation($"Attempting to update bookmark: {bookmark.Id}");

		var toUpdate = await _context.Bookmarks
			.FirstOrDefaultAsync(b => b.Id == bookmark.Id);

		if (toUpdate == null)
		{
			_logger.LogWarning($"Bookmark with ID: {bookmark.Id} not found.");
			return;
		}

		toUpdate.Title = bookmark.Title ?? toUpdate.Title;
		toUpdate.Url = bookmark.Url ?? toUpdate.Url;
		toUpdate.Description = bookmark.Description ?? toUpdate.Description;

		_context.Update(toUpdate);
		try
		{
			await _context.SaveChangesAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			throw;
		}
		_logger.LogInformation($"Bookmark with ID: {bookmark.Id} updated successfully.");
	}
}
