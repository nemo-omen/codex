using System.Data.Common;
using Respawn;

namespace Codex.Tests.Integration;

public class IntegrationTestBase
{
    protected readonly IServiceProvider ServiceProvider;
    protected readonly IntegrationTestFactory Factory;
    protected Respawner Respawner;
    protected DbConnection Connection;
    public readonly string TestUserId;

    public IntegrationTestBase(IntegrationTestFactory factory)
    {
        Factory = factory;
        ServiceProvider = factory.Services;
        Respawner = factory.Respawner;
        Connection = factory.Connection;
        TestUserId = factory.TestUserId;
    }

    public Task InitializeAsync() => Task.CompletedTask;
    public async Task DisposeAsync() => await Factory.ResetDatabaseAsync();
}