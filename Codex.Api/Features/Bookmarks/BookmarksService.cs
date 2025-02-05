using System;
using Codex.Api.Features.Bookmarks.Types;
using Codex.Api.Models;
using Microsoft.Build.Framework;

namespace Codex.Api.Features.Bookmarks;

public interface IBookmarksService
{
    Task<Guid> CreateBookmarkAsync(CreateBookmarkRequest request);
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

    public async Task<Guid> CreateBookmarkAsync(CreateBookmarkRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.UserId)) throw new ArgumentException("UserId is required.");

        var bookmark = new Bookmark
        {
            Id = Guid.NewGuid(),
            Title = request.Title,
            Url = request.Url,
            Description = request.Description,
            UserId = request.UserId,
        };

        return await _bookmarksRepository.CreateBookmarkAsync(bookmark);
    }
}
