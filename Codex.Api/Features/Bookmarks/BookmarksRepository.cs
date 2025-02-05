using System;
using Codex.Api.Data;
using Codex.Api.Models;

namespace Codex.Api.Features.Bookmarks;

public interface IBookmarksRepository
{
    Task<Guid> CreateBookmarkAsync(Bookmark bookmark);
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

    public async Task<Guid> CreateBookmarkAsync(Bookmark bookmark)
    {
        _context.Bookmarks.Add(bookmark);
        await _context.SaveChangesAsync();
        return bookmark.Id;
    }
}
