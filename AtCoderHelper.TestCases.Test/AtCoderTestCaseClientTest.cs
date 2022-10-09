using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TerryU16.AtCoderHelper.TestCases.Test;

public class AtCoderTestCaseClientTest
{
    [Fact]
    public async Task GetTestCasesAsyncTest()
    {
        var client = new AtCoderTestCaseClient();

        var testCases = await client.GetTestCasesAsync("abc271", "a");

        var expected = new TestCase[]
        {
            new TestCase("99", "63"),
            new TestCase("12", "0C"),
            new TestCase("0", "00"),
            new TestCase("255", "FF"),
        };

        Assert.Equal(expected, testCases);
    }
}
