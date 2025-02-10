using Codex.Api.Models;
using Codex.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace Codex.Api.Features.Collections;

public interface ICollectionsRepository
{
	public Task<List<CollectionEntity>> GetCollectionsAsync(string userId);
	public Task<CollectionEntity?> GetCollectionByIdAsync(Guid id);
	public Task<Guid> CreateCollectionAsync(CollectionEntity collection);
	public Task<bool> UpdateCollectionAsync(CollectionEntity collection);
	public Task<bool> DeleteCollectionAsync(Guid id);
}

public class CollectionsRepository : ICollectionsRepository
{
	private readonly ILogger<CollectionsRepository> _logger;
	private readonly ApplicationDbContext _context;

	public CollectionsRepository(ILogger<CollectionsRepository> logger, ApplicationDbContext context)
	{
		_logger = logger;
		_context = context;
	}

	public async Task<List<CollectionEntity>> GetCollectionsAsync(string userId)
	{
		_logger.LogInformation($"Attempting to retrieve collections for user: {userId}");
		var collections = await _context.Collections
			.AsNoTracking()
			.Include(c => c.Notes)
			.Include(c => c.Bookmarks)
			.Where(c => c.UserId == userId)
			.ToListAsync();

		_logger.LogInformation($"Successfully retrieved {collections.Count} collections for user: {userId}");
		return collections;
	}

	public async Task<CollectionEntity?> GetCollectionByIdAsync(Guid id)
	{
		_logger.LogInformation($"Attempting to retrieve collection: {id}");
		var collection = await _context.Collections
			.AsNoTracking()
			.Include(c => c.Notes)
			.Include(c => c.Bookmarks)
			.FirstOrDefaultAsync(c => c.Id == id);

		if (collection is null)
		{
			_logger.LogWarning($"Collection with ID: {id} not found.");
			return collection;
		}

		_logger.LogInformation($"Successfully retrieved collection: {id}");
		return collection;
	}

	public async Task<Guid> CreateCollectionAsync(CollectionEntity collection)
	{
		_logger.LogInformation($"Attempting to create collection: {collection.Id}");
		_context.Collections.Add(collection);
		await _context.SaveChangesAsync();
		_logger.LogInformation($"Collection created successfully with ID: {collection.Id}");
		return collection.Id;
	}

	public async Task<bool> UpdateCollectionAsync(CollectionEntity collection)
	{
		var existing = await _context.Collections
			.FirstOrDefaultAsync(c => c.Id == collection.Id);
		if (existing is null)
		{
			_logger.LogWarning($"Collection with ID: {collection.Id} not found.");
			return false;
		}

		_logger.LogInformation($"Attempting to update collection: {collection.Id}");
		existing.Name = collection.Name ?? existing.Name;
		existing.Description = collection.Description ?? existing.Description;
		existing.Notes = collection.Notes ?? existing.Notes;
		existing.Bookmarks = collection.Bookmarks ?? existing.Bookmarks;

		await _context.SaveChangesAsync();
		_logger.LogInformation($"Collection updated successfully with ID: {collection.Id}");
		return true;
	}

	public async Task<bool> DeleteCollectionAsync(Guid id)
	{
		_logger.LogInformation($"Attempting to delete collection: {id}");
		var collection = await _context.Collections
			.FirstOrDefaultAsync(c => c.Id == id);

		if (collection is null)
		{
			_logger.LogWarning($"Collection with ID: {id} not found.");
			return false;
		}
		_context.Collections.Remove(collection);
		await _context.SaveChangesAsync();
		_logger.LogInformation($"Collection deleted successfully with ID: {id}");
		return true;
	}
}
