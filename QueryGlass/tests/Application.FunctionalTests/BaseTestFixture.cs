
using static QueryGlass.Application.FunctionalTests.Testing;

namespace QueryGlass.Application.FunctionalTests;

[TestFixture]
public abstract class BaseTestFixture
{
    [SetUp]
    public async Task TestSetUp()
    {
        await ResetState();
    }
}
