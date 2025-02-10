using Codex.Api.Features.Collections.Types;
using Codex.Api.Models;

namespace Codex.Api.Features.Collections.Mappers;

public static class CollectionMapper
{
	public static CollectionResponse ToCollectionResponse(CollectionEntity collection)
	{
		return new CollectionResponse
		{
			Id = collection.Id,
			Name = collection.Name,
			Description = collection.Description,
			UserId = collection.UserId,
			Notes = collection.Notes.Select(n => n.Id).ToList(),
			Bookmarks = collection.Bookmarks.Select(b => b.Id).ToList()
		};
	}

	public static CollectionEntity FromCreateCollectionRequest(CreateCollectionRequest request)
	{
		return new CollectionEntity
		{
			Id = Guid.NewGuid(),
			UserId = request.UserId,
			Name = request.Name,
			Description = request.Description,
		};
	}

	public static CollectionEntity FromUpdateCollectionRequest(UpdateCollectionRequest request)
	{
		return new CollectionEntity
		{
			Id = request.Id,
			UserId = request.UserId,
			Name = request.Name ?? "",
			Description = request.Description,
			Bookmarks = [],
			Notes = []
		};
	}
}
