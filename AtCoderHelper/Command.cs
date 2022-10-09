using TerryU16.AtCoderHelper.CodeFormatters;
using TerryU16.AtCoderHelper.TestCases;

namespace TerryU16.AtCoderHelper;

internal class Command : ConsoleAppBase
{
    [Command("test", "get test cases.")]
    public async Task GetTestCases([Option("c", "contest name")] string contestName, [Option("p", "problem name")] string problemName)
    {
        var testCaseManager = new TestCaseManager();
        await testCaseManager.GetTestCaseAsync(contestName, problemName, Context.CancellationToken);
    }

    [Command("format", "format code for submit.")]
    public async Task FormatCode([Option("p", "problem name")] string problemName)
    {
        var formatter = new CodeFormatter();
        await formatter.ConcatAsync(problemName, Context.CancellationToken);
    }
}
