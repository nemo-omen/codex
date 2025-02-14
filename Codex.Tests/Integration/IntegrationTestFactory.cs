using System.Data.Common;
using Codex.Api.Data;
using Codex.Api.Features.Bookmarks;
using Codex.Api.Features.Collections;
using Codex.Api.Features.Notes;
using Codex.Api.Models;
using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Respawn;
using Testcontainers.PostgreSql;

namespace Codex.Tests.Integration;

public class IntegrationTestFactory : WebApplicationFactory<Program>, IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer;
    public ApplicationDbContext Context { get; set; } = null!;
    public string TestUserId { get; set; }  = "codex-test-user-id";
    // public IServiceProvider ServiceProvider { get; set; } = null!;
    public DbConnection Connection = null!;
    public Respawner Respawner = null!;

    public HttpClient HttpClient { get; private set; } = null!;

    public IntegrationTestFactory()
    {
        _dbContainer = new PostgreSqlBuilder()
            .WithImage("postgres:latest")
            .WithDatabase("codex_test")
            .WithUsername("codex_test_admin")
            .WithPassword("codex!")
            .WithExposedPort(5432)
            .WithWaitStrategy(Wait.ForUnixContainer().UntilPortIsAvailable(5432))
            .WithCleanUp(true)
            .Build();
    }

    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        var host = _dbContainer.Hostname;
        var port = _dbContainer.GetMappedPublicPort(5432);

        builder.ConfigureServices(services =>
        {
            services.RemoveDbContext<ApplicationDbContext>();
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql($"Host={host};Port={port};Database=codex_test;Username=codex_test_admin;Password=codex!"));
            services.EnsureDbCreated<ApplicationDbContext>();

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
    }

    public async Task InitializeAsync()
    {
        await _dbContainer.StartAsync();

        HttpClient = CreateClient();

        Context = Services.CreateScope().ServiceProvider.GetRequiredService<ApplicationDbContext>();
        await Context.Database.EnsureDeletedAsync();
        await Context.Database.MigrateAsync();
        await SeedTestUser(Services);
        Connection = Context.Database.GetDbConnection();
        await Connection.OpenAsync();
        Respawner = await Respawner.CreateAsync(Connection, new RespawnerOptions
        {
            DbAdapter = DbAdapter.Postgres,
            SchemasToInclude = [ "public" ]
        });
    }

    public async Task DisposeAsync()
    {
        await Context.Database.EnsureDeletedAsync();
        await Connection.CloseAsync();
        await _dbContainer.StopAsync();
        await _dbContainer.DisposeAsync();
    }

    public async Task ResetDatabaseAsync()
    {
        await Respawner.ResetAsync(Connection);
    }

    public async Task SeedTestUser(IServiceProvider serviceProvider)
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

public static class ServiceCollectionExtensions
{
    public static void RemoveDbContext<T>(this IServiceCollection services) where T : DbContext
    {
        var descriptor = services.SingleOrDefault(x => x.ServiceType == typeof(DbContextOptions<T>));
        if (descriptor != null)
        {
            services.Remove(descriptor);
        }
    }

    public static void EnsureDbCreated<T>(this IServiceCollection services) where T : DbContext
    {
        using var scope = services.BuildServiceProvider().CreateScope();
        var serviceProvider = scope.ServiceProvider;
        var context = serviceProvider.GetRequiredService<T>();
        context.Database.EnsureCreated();
    }
}