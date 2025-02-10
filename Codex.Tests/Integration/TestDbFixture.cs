using Codex.Api.Data;
using Codex.Api.Features.Bookmarks;
using Codex.Api.Features.Collections;
using Codex.Api.Features.Notes;
using Codex.Api.Models;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using TestContainers.Container.Abstractions.Hosting;
using Testcontainers.PostgreSql;


namespace Codex.Tests.Integration;

public class TestDbFixture : IAsyncLifetime
{
    public PostgreSqlContainer DbContainer { get; private set; }
    public WebApplicationFactory<Program> Factory { get; private set; }
    public IServiceProvider ServiceProvider => Factory.Services;
    public HttpClient Client { get; private set; }
    public string TestUserId { get; set; }  = "codex-test-user-id";

    public TestDbFixture()
    {
        DbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("codex_test")
            .WithUsername("codex_test_admin")
            .WithPassword("codex!")
            .WithExposedPort(5432)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .WithCleanUp(true)
            .Build();
    }

    public async Task InitializeAsync()
    {
        try
        {
            Console.WriteLine("Starting PostgreSQL TestContainer...");
            await DbContainer.StartAsync();
            Console.WriteLine("PostgreSQL TestContainer started.");
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to start test container: {e.Message}");
            throw;
        }

        Factory = new WebApplicationFactory<Program>()
            .WithWebHostBuilder(builder =>
            {
                builder.ConfigureTestServices(services =>
                {
                    services.AddDbContext<ApplicationDbContext>(options =>
                        options.UseNpgsql(DbContainer.GetConnectionString()));

                    services.AddLogging();
                    services.AddScoped<INotesRepository, NotesRepository>();
                    services.AddScoped<IBookmarksRepository, BookmarksRepository>();
                    services.AddScoped<ICollectionsRepository, CollectionsRepository>();

                    services.AddScoped<INotesService, NotesService>();
                    services.AddScoped<IBookmarksService, BookmarksService>();
                    services.AddScoped<ICollectionsService, CollectionsService>();

                    services.AddAuthentication(options =>
                    {
                        options.DefaultAuthenticateScheme = "Test";
                        options.DefaultChallengeScheme = "Test";
                        options.DefaultScheme = "Test";
                    })
                    .AddScheme<TestAuthHandlerOptions, TestAuthHandler>("Test", options => { });
                });
            });

        Client = Factory.CreateClient();
        using var scope = ServiceProvider.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        try
        {
            Console.WriteLine("Migrating database...");
            await dbContext.Database.EnsureDeletedAsync();
            await dbContext.Database.MigrateAsync();
            Console.WriteLine("Database migrated sucessfully.");

            await SeedTestUser(ServiceProvider);
        }
        catch (Exception e)
        {
            Console.WriteLine($"Failed to migrate database: {e.Message}");
            throw;
        }
    }

    public async Task DisposeAsync()
    {
        await DbContainer.StopAsync();
        await DbContainer.DisposeAsync();
    }

    private async Task SeedTestUser(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

        var testUserEmail = "test@test.test";

        var existingUser = await userManager.FindByIdAsync(TestUserId);
        if (existingUser is null)
        {
            var testUser = new ApplicationUser
            {
                Id = TestUserId,
                Email = testUserEmail,
                UserName = "test",
                EmailConfirmed = true
            };

            var testPassword = "TestPassword123!";
            var result = await userManager.CreateAsync(testUser, testPassword);

            if (!result.Succeeded)
            {
                throw new Exception($"Failed to create test user: {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }

            Console.WriteLine($"Test user created: {TestUserId}");
        }
    }
}