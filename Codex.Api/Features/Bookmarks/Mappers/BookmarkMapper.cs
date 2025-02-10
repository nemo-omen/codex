using System;
using Codex.Api.Features.Bookmarks.Types;
using Codex.Api.Features.Notes;
using Codex.Api.Features.Notes.Types;
using Codex.Api.Models;

namespace Codex.Api.Features.Bookmarks.Mappers;

public static class BookmarkMapper
{
	public static BookmarkEntity FromBookmarkDto(BookmarkDto bookmarkDto)
	{
		return new BookmarkEntity
		{
			Id = bookmarkDto.Id,
			Title = bookmarkDto.Title,
			Url = bookmarkDto.Url,
			Description = bookmarkDto.Description,
			UserId = bookmarkDto.UserId,
			CollectionId = bookmarkDto.CollectionId,
			Notes = bookmarkDto.Notes.Select(NotesMapper.FromDtoToEntity).ToList(),
		};
	}

	public static BookmarkEntity FromUpdateBookmarkRequest(UpdateBookmarkRequest request)
	{
		return new BookmarkEntity
		{
			Id = request.Id,
			Title = request.Title,
			Url = request.Url,
			Description = request.Description,
			UserId = request.UserId,
			CollectionId = (Guid)request.CollectionId,
			Notes = request.Notes.Select(NotesMapper.FromDtoToEntity).ToList(),
		};
	}
}
