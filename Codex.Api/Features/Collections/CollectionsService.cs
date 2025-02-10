using System;
using Codex.Api.Exceptions;
using Codex.Api.Features.Bookmarks;
using Codex.Api.Features.Collections.Mappers;
using Codex.Api.Features.Collections.Types;
using Codex.Api.Features.Notes;
using Codex.Api.Models;
using EntityFramework.Exceptions.Common;

namespace Codex.Api.Features.Collections;

public interface ICollectionsService
{
	Task<List<CollectionResponse>> GetCollectionsAsync(string userId);
	Task<CollectionResponse?> GetCollectionByIdAsync(Guid id);
	Task<Guid> CreateCollectionAsync(CreateCollectionRequest request);
	Task<bool> UpdateCollectionAsync(UpdateCollectionRequest request);
	Task<bool> DeleteCollectionAsync(Guid collectionId);
}

public class CollectionsService : ICollectionsService
{
	private readonly ILogger<CollectionsService> _logger;
	private readonly ICollectionsRepository _collectionsRepository;
	private readonly INotesRepository _notesRepository;
	private readonly IBookmarksRepository _bookmarksRepository;

	public CollectionsService(ILogger<CollectionsService> logger,
		ICollectionsRepository collectionsRepository, INotesRepository notesRepository,
		IBookmarksRepository bookmarksRepository)
	{
		_logger = logger;
		_collectionsRepository = collectionsRepository;
		_notesRepository = notesRepository;
		_bookmarksRepository = bookmarksRepository;
	}

	public async Task<List<CollectionResponse>> GetCollectionsAsync(string userId)
	{
		var collections = await _collectionsRepository.GetCollectionsAsync(userId);
		return collections
			.Select(CollectionMapper.ToCollectionResponse)
			.ToList();
	}

	public async Task<CollectionResponse?> GetCollectionByIdAsync(Guid id)
	{
		var collection = await _collectionsRepository.GetCollectionByIdAsync(id);

		if (collection is null) return null;

		return CollectionMapper.ToCollectionResponse(collection);
	}

	public async Task<Guid> CreateCollectionAsync(CreateCollectionRequest request)
	{
		var collection = CollectionMapper.FromCreateCollectionRequest(request);

		try
		{
			var result = await _collectionsRepository.CreateCollectionAsync(collection);
			return result;
		}
		catch (UniqueConstraintException e)
		{
			_logger.LogError(e.Message);
			throw new ConflictException("Collection with the same name and user Id already exists.");
		}
	}

	public async Task<bool> UpdateCollectionAsync(UpdateCollectionRequest request)
	{
		var collection = CollectionMapper.FromUpdateCollectionRequest(request);

		foreach (var noteId in request.Notes)
		{
			var note = await _notesRepository.GetNoteByIdAsync(Guid.Parse(noteId));
			if (note is null) throw new NotFoundException("Note not found.");
			collection.Notes.Add(note);
			note.CollectionId = collection.Id;
			var saveNoteResult = await _notesRepository.UpdateNoteAsync(note);
		}

		foreach (var bookmarkId in request.Bookmarks)
		{
			var bookmark = await _bookmarksRepository.GetBookmarkByIdAsync(Guid.Parse(bookmarkId));
			if (bookmark is null) throw new NotFoundException("Bookmark not found.");
			collection.Bookmarks.Add(bookmark);
			bookmark.CollectionId = collection.Id;
			var saveBookmarkResult = await _bookmarksRepository.UpdateBookmarkAsync(bookmark);
		}

		try
		{
			var result = await _collectionsRepository.UpdateCollectionAsync(collection);
			if (!result) throw new NotFoundException("Collection not found.");
			return result;
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			throw;
		}
	}

	public async Task<bool> DeleteCollectionAsync(Guid collectionId)
	{
		var collection = await _collectionsRepository.GetCollectionByIdAsync(collectionId);
		if (collection is null) throw new NotFoundException("Collection not found.");

		foreach (var note in collection.Notes)
		{
			note.CollectionId = null;
			note.Collection = null;
			collection.Notes.Remove(note);
			await _notesRepository.UpdateNoteAsync(note);
		}

		foreach (var bookmark in collection.Bookmarks)
		{
			bookmark.CollectionId = null;
			bookmark.Collection = null;
			collection.Bookmarks.Remove(bookmark);
			await _bookmarksRepository.UpdateBookmarkAsync(bookmark);
		}

		try
		{
			var result = await _collectionsRepository.DeleteCollectionAsync(collectionId);
			if (!result) return false;

			return true;
		}
		catch (Exception e)
		{
			_logger.LogError(e.Message);
			return false;
		}
	}
}
