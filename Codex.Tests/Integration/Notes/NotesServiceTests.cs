using Codex.Api.Data;
using Codex.Api.Exceptions;
using Codex.Api.Features.Bookmarks;
using Codex.Api.Features.Notes;
using Codex.Api.Features.Notes.Types;
using Codex.Api.Models;
using Microsoft.Extensions.DependencyInjection;
using Xunit;

namespace Codex.Tests.Integration.Notes;

[Collection("Test DB Collection")]
public class NotesServiceTests
{
    private readonly IServiceProvider _serviceProvider;
    private readonly string _testUserId;

    public NotesServiceTests(TestDbFixture fixture)
    {
        _serviceProvider = fixture.ServiceProvider;
        _testUserId = fixture.TestUserId;
    }

    [Fact]
    public async Task NotesService_CreateNoteAsync_ShouldPersistNote()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var notesService = scope.ServiceProvider.GetRequiredService<INotesService>();
        var transaction = await context.Database.BeginTransactionAsync();

        var request = new CreateNoteRequest
        {
            Title = "Test Note",
            Content = "Test Content",
            UserId = _testUserId, // ✅ Use seeded test user
        };

        var noteId = await notesService.CreateNoteAsync(request);
        var retrievedNote = await notesService.GetNoteByIdAsync(noteId);

        Assert.NotNull(retrievedNote);
        Assert.Equal(noteId, retrievedNote.Id);
        Assert.Equal(request.Title, retrievedNote.Title);
        Assert.Equal(request.Content, retrievedNote.Content);
        Assert.Equal(request.UserId, retrievedNote.UserId);
        await transaction.RollbackAsync();
        await transaction.DisposeAsync();
    }

    [Fact]
    public async Task NotesService_UpdateNoteAsync_ShouldModifyNote()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var notesService = scope.ServiceProvider.GetRequiredService<INotesService>();
        var transaction = await context.Database.BeginTransactionAsync();

        // Create a note first
        var createRequest = new CreateNoteRequest
        {
            Title = "Original Title",
            Content = "Original Content",
            UserId = _testUserId,
        };
        var noteId = await notesService.CreateNoteAsync(createRequest);

        // Update the note
        var updateRequest = new EditNoteRequest
        {
            Id = noteId,
            Title = "Updated Title",
            Content = "Updated Content",
            UserId = _testUserId
        };
        await notesService.UpdateNoteAsync(updateRequest);

        // Retrieve updated note
        var updatedNote = await notesService.GetNoteByIdAsync(noteId);

        Assert.Equal("Updated Title", updatedNote.Title);
        Assert.Equal("Updated Content", updatedNote.Content);
        await transaction.RollbackAsync();
        await transaction.DisposeAsync();
    }

    [Fact]
    public async Task NotesService_DeleteNoteAsync_ShouldRemoveNote()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var notesService = scope.ServiceProvider.GetRequiredService<INotesService>();
        var transaction = await context.Database.BeginTransactionAsync();

        // Create a note first
        var createRequest = new CreateNoteRequest
        {
            Title = "To be deleted",
            Content = "Content",
            UserId = _testUserId,
        };
        var noteId = await notesService.CreateNoteAsync(createRequest);

        // Delete the note
        await notesService.DeleteNoteAsync(noteId);

        // Ensure note no longer exists
        await Assert.ThrowsAsync<NotFoundException>(() => notesService.GetNoteByIdAsync(noteId));
        await transaction.RollbackAsync();
        await transaction.DisposeAsync();
    }

    [Fact]
    public async Task NotesService_GetNotesAsync_ShouldReturnUserNotes()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var notesService = scope.ServiceProvider.GetRequiredService<INotesService>();
        var transaction = await context.Database.BeginTransactionAsync();

        // Create two notes
        await notesService.CreateNoteAsync(new CreateNoteRequest
        {
            Title = "First Note",
            Content = "Content 1",
            UserId = _testUserId,
        });

        await notesService.CreateNoteAsync(new CreateNoteRequest
        {
            Title = "Second Note",
            Content = "Content 2",
            UserId = _testUserId,
        });

        var notes = await notesService.GetNotesAsync(_testUserId);

        Assert.Equal(2, notes.Count);
        Assert.Contains(notes, n => n.Title == "First Note");
        Assert.Contains(notes, n => n.Title == "Second Note");
        await transaction.RollbackAsync();
        await transaction.DisposeAsync();
    }
}