using Xunit;
namespace Codex.Tests.Integration;

[CollectionDefinition("Test DB Collection")]
public class TestDbCollection : ICollectionFixture<TestDbFixture>
{

}