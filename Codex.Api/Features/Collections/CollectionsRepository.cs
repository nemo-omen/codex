using Codex.Api.Models;
using Codex.Api.Data;
using Microsoft.EntityFrameworkCore;
using Codex.Api.Exceptions;

namespace Codex.Api.Features.Collections;

public interface ICollectionsRepository
{
	public Task<List<Collection>> GetCollectionsAsync(string userId);
	public Task<Collection?> GetCollectionByIdAsync(Guid id);
	public Task<Guid> CreateCollectionAsync(Collection collection);
	public Task UpdateCollectionAsync(Collection collection);
	public Task DeleteCollectionAsync(Guid id);
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

	public async Task<List<Collection>> GetCollectionsAsync(string userId)
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

	public async Task<Collection> GetCollectionAsync(Guid id)
	{
		_logger.LogInformation($"Attempting to retrieve collection: {id}");
		var collection = await _context.Collections
			.AsNoTracking()
			.Include(c => c.Notes)
			.Include(c => c.Bookmarks)
			.FirstOrDefaultAsync(c => c.Id == id) ??
				throw new NotFoundException("Collection not found.");

		_logger.LogInformation($"Successfully retrieved collection: {id}");

		return collection;
	}

	public async Task<Collection?> GetCollectionByIdAsync(Guid id)
	{
		_logger.LogInformation($"Attempting to retrieve collection: {id}");
		var collection = await _context.Collections
			.AsNoTracking()
			.Include(c => c.Notes)
			.Include(c => c.Bookmarks)
			.FirstOrDefaultAsync(c => c.Id == id);

		if (collection == null) throw new NotFoundException("Collection not found.");

		_logger.LogInformation($"Successfully retrieved collection: {id}");
		return collection;
	}

	public async Task<Guid> CreateCollectionAsync(Collection collection)
	{
		_logger.LogInformation($"Attempting to create collection: {collection.Id}");
		_context.Collections.Add(collection);
		await _context.SaveChangesAsync();
		_logger.LogInformation($"Collection created successfully with ID: {collection.Id}");
		return collection.Id;
	}

	public async Task UpdateCollectionAsync(Collection collection)
	{
		_logger.LogInformation($"Attempting to update collection: {collection.Id}");
		_context.Collections.Update(collection);
		await _context.SaveChangesAsync();
		_logger.LogInformation($"Collection updated successfully with ID: {collection.Id}");
	}

	public async Task DeleteCollectionAsync(Guid id)
	{
		_logger.LogInformation($"Attempting to delete collection: {id}");
		var collection = await GetCollectionAsync(id);
		if (collection == null) throw new NotFoundException("Collection not found.");

		_context.Collections.Remove(collection);
		await _context.SaveChangesAsync();
		_logger.LogInformation($"Collection deleted successfully with ID: {id}");
	}
}
