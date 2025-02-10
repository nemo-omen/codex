using Codex.Api.Data;
using Codex.Api.Exceptions;
using Codex.Api.Features.Bookmarks;
using Codex.Api.Features.Notes;
using Codex.Api.Models;
using Microsoft.Extensions.DependencyInjection;

namespace Codex.Tests.Integration.Notes;

[Collection("Test DB Collection")]
public class NotesRepositoryTests
{
    // private readonly INotesRepository _notesRepository;
    private readonly IServiceProvider _serviceProvider;
    private readonly string _testUserId;

    public NotesRepositoryTests(TestDbFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
        _testUserId = fixture.TestUserId;
    }


    [Fact]
    public async Task NotesRepository_CreateNoteAsync_ShouldPersistNote()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var notesRepository = scope.ServiceProvider.GetRequiredService<INotesRepository>();
        var transaction = await context.Database.BeginTransactionAsync();

        var note = new NoteEntity
        {
            UserId = _testUserId,
            Id = Guid.NewGuid(),
            Title = "Test Note",
            Content = "Test Content",
        };

        var noteId = await notesRepository.CreateNoteAsync(note);
        var retrievedNote = await notesRepository.GetNoteByIdAsync(noteId);

        Assert.NotNull(retrievedNote);
        Assert.Equal(noteId, retrievedNote.Id);
        Assert.Equal(note.Title, retrievedNote.Title);
        Assert.Equal(note.Content, retrievedNote.Content);
        Assert.Equal(note.BookmarkId, retrievedNote.BookmarkId);
        Assert.Equal(note.CollectionId, retrievedNote.CollectionId);

        await transaction.RollbackAsync();
        await transaction.DisposeAsync();
    }

    [Fact]
    public async Task NotesRepository_GetNotesAsync_ShouldReturnNotes()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var notesRepository = scope.ServiceProvider.GetRequiredService<INotesRepository>();
        var transaction = await context.Database.BeginTransactionAsync();

        var note = new NoteEntity
        {
            UserId = _testUserId,
            Id = Guid.NewGuid(),
            Title = "Test Note",
            Content = "Test Content",
        };

        var note2 = new NoteEntity
        {
            UserId = _testUserId,
            Id = Guid.NewGuid(),
            Title = "Test Note 2",
            Content = "Test Content 2",
        };

        await notesRepository.CreateNoteAsync(note);
        await notesRepository.CreateNoteAsync(note2);

        var notes = await notesRepository.GetNotesAsync(_testUserId);

        Assert.Equal(2, notes.Count);
        await transaction.RollbackAsync();
        await transaction.DisposeAsync();
    }

    [Fact]
    public async Task NotesRepository_GetNoteByIdAsync_ShouldReturnNote()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var notesRepository = scope.ServiceProvider.GetRequiredService<INotesRepository>();
        var transaction = await context.Database.BeginTransactionAsync();

        var note = new NoteEntity
        {
            UserId = _testUserId,
            Id = Guid.NewGuid(),
            Title = "Test Note",
            Content = "Test Content",
        };

        await notesRepository.CreateNoteAsync(note);

        var retrievedNote = await notesRepository.GetNoteByIdAsync(note.Id);

        Assert.NotNull(retrievedNote);
        Assert.Equal(note.Id, retrievedNote.Id);
        Assert.Equal(note.Title, retrievedNote.Title);
        Assert.Equal(note.Content, retrievedNote.Content);
        await transaction.RollbackAsync();
        await transaction.DisposeAsync();
    }

    [Fact]
    public async Task NoteRepository_UpdateNoteAsync_ShouldUpdateNote()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var notesRepository = scope.ServiceProvider.GetRequiredService<INotesRepository>();
        var transaction = await context.Database.BeginTransactionAsync();

        var note = new NoteEntity
        {
            UserId = _testUserId,
            Id = Guid.NewGuid(),
            Title = "Test Note",
            Content = "Test Content",
        };

        await notesRepository.CreateNoteAsync(note);

        var updatedNote = new NoteEntity
        {
            UserId = _testUserId,
            Id = note.Id,
            Title = "Updated Note",
            Content = "Updated Content",
        };

        await notesRepository.UpdateNoteAsync(updatedNote);

        var retrievedNote = await notesRepository.GetNoteByIdAsync(note.Id);

        Assert.NotNull(retrievedNote);
        Assert.Equal(updatedNote.Id, retrievedNote.Id);
        Assert.Equal(updatedNote.Title, retrievedNote.Title);
        Assert.Equal(updatedNote.Content, retrievedNote.Content);
        await transaction.RollbackAsync();
        await transaction.DisposeAsync();
    }

    [Fact]
    public async Task NoteRepository_DeleteNoteAsync_ShouldDeleteNote()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var notesRepository = scope.ServiceProvider.GetRequiredService<INotesRepository>();
        var transaction = await context.Database.BeginTransactionAsync();

        var note = new NoteEntity
        {
            UserId = _testUserId,
            Id = Guid.NewGuid(),
            Title = "Test Note",
            Content = "Test Content",
        };

        await notesRepository.CreateNoteAsync(note);

        await notesRepository.DeleteNoteAsync(note.Id);

        await Assert.ThrowsAsync<NotFoundException>(async () =>
            await notesRepository.GetNoteByIdAsync(note.Id));

        await transaction.RollbackAsync();
        await transaction.DisposeAsync();
    }
}