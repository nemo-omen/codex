using Codex.Api.Data;
using Codex.Api.Exceptions;
using Codex.Api.Models;
using Codex.Api.Shared;
using Microsoft.EntityFrameworkCore;

namespace Codex.Api.Features.Bookmarks;

public interface IBookmarksRepository : IRepositoryBase<BookmarkEntity>
{
	Task<List<BookmarkEntity>> GetBookmarksAsync(string userId);
	Task<BookmarkEntity?> GetBookmarkByIdAsync(Guid id);
	Task<BookmarkEntity?> GetBookmarkByUrlAsync(string url);
	Task<Guid> CreateBookmarkAsync(BookmarkEntity bookmark);
	Task<bool> UpdateBookmarkAsync(BookmarkEntity bookmark);
	Task DeleteBookmarkAsync(Guid id);
}

public class BookmarksRepository : RepositoryBase<BookmarkEntity>, IBookmarksRepository
{
	private readonly ILogger<BookmarksRepository> _logger;

	public BookmarksRepository(ILogger<BookmarksRepository> logger, ApplicationDbContext context)
		: base(context)
	{
		_logger = logger;
	}


	/// <summary>
	/// Retrieves all bookmarks for the given user ID.
	/// </summary>
	/// <param name="userId">The ID of the user to retrieve bookmarks for.</param>
	/// <returns>A list of bookmarks for the given user.</returns>
	public async Task<List<BookmarkEntity>> GetBookmarksAsync(string userId)
	{
		_logger.LogInformation($"Attempting to retrieve bookmarks for user: {userId}");
		var bookmarks = await FindByCondition(b => string.Equals(b.UserId, userId))
			.Include(b => b.Notes)
			.ToListAsync();

		_logger.LogInformation($"Successfully retrieved {bookmarks.Count} bookmarks for user: {userId}");
		return bookmarks;
	}

	/// <summary>
	/// Retrieves a bookmark from the database with the given ID, or null if it does not exist.
	/// </summary>
	/// <param name="id">The ID of the bookmark to retrieve.</param>
	/// <returns>The bookmark with the given ID if it exists, or null.</returns>
	public async Task<BookmarkEntity?> GetBookmarkByIdAsync(Guid id)
	{
		_logger.LogInformation($"Attempting to retrieve bookmark with ID: {id}");
		var bookmark = await FindByCondition(b => b.Id == id)
			.Include(b => b.Notes)
			.FirstOrDefaultAsync();

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
	public async Task<BookmarkEntity?> GetBookmarkByUrlAsync(string url)
	{
		_logger.LogInformation($"Attempting to retrieve bookmark with URL: {url}");
		var bookmark = await FindByCondition(b => string.Equals(b.Url, url))
			.Include(b => b.Notes)
			.FirstOrDefaultAsync();

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
	public async Task<Guid> CreateBookmarkAsync(BookmarkEntity bookmark)
	{
		_logger.LogInformation($"Attempting to create bookmark: {bookmark}");
		Create(bookmark);
		await RepositoryContext.SaveChangesAsync();
		_logger.LogInformation($"Bookmark created successfully with ID: {bookmark.Id}");
		return bookmark.Id;
	}

	/// <summary>
	/// Updates an existing bookmark in the database with the provided data.
	/// </summary>
	/// <param name="bookmark">The bookmark data to update, including the ID of the bookmark to be updated.</param>
	/// <exception cref="ArgumentException">Thrown when the ID of the bookmark is not provided.</exception>
	/// <exception cref="NotFoundException">Thrown when the a bookmark with the provided ID is not found.</exception>
	/// <remarks>If the bookmark with the given ID does not exist, a warning is logged and no update is performed.</remarks>
	public async Task<bool> UpdateBookmarkAsync(BookmarkEntity bookmark)
	{
		if (bookmark.Id == Guid.Empty) return false;

		_logger.LogInformation($"Attempting to update bookmark: {bookmark.Id}");

		var toUpdate = await FindByCondition(b => b.Id == bookmark.Id)
			.FirstOrDefaultAsync();

		if (toUpdate == null)
		{
			_logger.LogWarning($"Bookmark with ID: {bookmark.Id} not found.");
			return false;
		}

		toUpdate.Title = bookmark.Title ?? toUpdate.Title;
		toUpdate.Url = bookmark.Url ?? toUpdate.Url;
		toUpdate.Description = bookmark.Description ?? toUpdate.Description;

		Update(toUpdate);
		await RepositoryContext.SaveChangesAsync();
		_logger.LogInformation($"Bookmark with ID: {bookmark.Id} updated successfully.");
		return true;
	}

	/// <summary>
	/// Deletes the bookmark with the given ID from the database.
	/// </summary>
	/// <param name="id">The ID of the bookmark to delete.</param>
	/// <exception cref="NotFoundException">Thrown if the bookmark with the given ID does not exist.</exception>
	/// <remarks>If the bookmark with the given ID does not exist, a warning is logged and no deletion is performed.</remarks>
	public async Task DeleteBookmarkAsync(Guid id)
	{
		_logger.LogInformation($"Attempting to delete bookmark with ID: {id}");
		var bookmark = await FindByCondition(b => b.Id == id)
			.FirstOrDefaultAsync();

		if (bookmark == null)
		{
			_logger.LogError($"Bookmark with id {id} not found.");
			throw new NotFoundException("Bookmark not found.");
		}

		Delete(bookmark);
		try
		{
			await RepositoryContext.SaveChangesAsync();
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			throw;
		}
		_logger.LogInformation($"Bookmark with ID: {id} deleted successfully.");
	}
}
