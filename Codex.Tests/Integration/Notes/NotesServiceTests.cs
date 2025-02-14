using System.Data.Common;
using Codex.Api.Data;
using Codex.Api.Exceptions;
using Codex.Api.Features.Bookmarks;
using Codex.Api.Features.Notes;
using Codex.Api.Features.Notes.Types;
using Codex.Api.Models;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Respawn;
using Xunit;

namespace Codex.Tests.Integration.Notes;

[Collection("Test DB Collection")]
public class NotesServiceTests(IntegrationTestFactory factory) : IntegrationTestBase(factory)
{
    [Fact]
    public async Task NotesService_CreateNoteAsync_ShouldPersistNote()
    {
        await Factory.SeedTestUser(ServiceProvider);
        using var scope = ServiceProvider.CreateScope();
        var notesService = scope.ServiceProvider.GetRequiredService<INotesService>();

        var request = new CreateNoteRequest
        {
            Title = "Test Note",
            Content = "Test Content",
            UserId = TestUserId, // ✅ Use seeded test user
        };

        var noteId = await notesService.CreateNoteAsync(request);
        var retrievedNote = await notesService.GetNoteByIdAsync(noteId);

        Assert.NotNull(retrievedNote);
        Assert.Equal(noteId, retrievedNote.Id);
        Assert.Equal(request.Title, retrievedNote.Title);
        Assert.Equal(request.Content, retrievedNote.Content);
        Assert.Equal(request.UserId, retrievedNote.UserId);
        await Respawner.ResetAsync(Connection);
    }

    [Fact]
    public async Task NotesService_UpdateNoteAsync_ShouldModifyNote()
    {
        await Factory.SeedTestUser(ServiceProvider);
        using var scope = ServiceProvider.CreateScope();
        var notesService = scope.ServiceProvider.GetRequiredService<INotesService>();

        // Create a note first
        var createRequest = new CreateNoteRequest
        {
            Title = "Original Title",
            Content = "Original Content",
            UserId = TestUserId,
        };
        var noteId = await notesService.CreateNoteAsync(createRequest);

        // Update the note
        var updateRequest = new EditNoteRequest
        {
            Id = noteId,
            Title = "Updated Title",
            Content = "Updated Content",
            UserId = TestUserId
        };
        await notesService.UpdateNoteAsync(updateRequest);

        // Retrieve updated note
        var updatedNote = await notesService.GetNoteByIdAsync(noteId);

        Assert.Equal("Updated Title", updatedNote.Title);
        Assert.Equal("Updated Content", updatedNote.Content);
        await Respawner.ResetAsync(Connection);
    }

    [Fact]
    public async Task NotesService_DeleteNoteAsync_ShouldRemoveNote()
    {
        await Factory.SeedTestUser(ServiceProvider);
        using var scope = ServiceProvider.CreateScope();
        var notesService = scope.ServiceProvider.GetRequiredService<INotesService>();

        // Create a note first
        var createRequest = new CreateNoteRequest
        {
            Title = "To be deleted",
            Content = "Content",
            UserId = TestUserId,
        };
        var noteId = await notesService.CreateNoteAsync(createRequest);

        // Delete the note
        await notesService.DeleteNoteAsync(noteId);

        // Ensure note no longer exists
        await Assert.ThrowsAsync<NotFoundException>(() => notesService.GetNoteByIdAsync(noteId));
        await Respawner.ResetAsync(Connection);
    }

    [Fact]
    public async Task NotesService_GetNotesAsync_ShouldReturnUserNotes()
    {
        await Factory.SeedTestUser(ServiceProvider);
        using var scope = ServiceProvider.CreateScope();
        var notesService = scope.ServiceProvider.GetRequiredService<INotesService>();

        // Create two notes
        await notesService.CreateNoteAsync(new CreateNoteRequest
        {
            Title = "First Note",
            Content = "Content 1",
            UserId = TestUserId,
        });

        await notesService.CreateNoteAsync(new CreateNoteRequest
        {
            Title = "Second Note",
            Content = "Content 2",
            UserId = TestUserId,
        });

        var notes = await notesService.GetNotesAsync(TestUserId);

        Assert.Equal(2, notes.Count);
        Assert.Contains(notes, n => n.Title == "First Note");
        Assert.Contains(notes, n => n.Title == "Second Note");
        await Respawner.ResetAsync(Connection);
    }
}