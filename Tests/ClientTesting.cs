using Microsoft.Extensions.Logging;
using Xunit;

namespace MiniMazErpBack.Tests;

public class ClientTesting
{

    [Fact]
    public void Test1()
    {
        // Test de ejemplo
        var expected = 1;
        var actual = 1;
        Assert.Equal(expected, actual);
    }
}
