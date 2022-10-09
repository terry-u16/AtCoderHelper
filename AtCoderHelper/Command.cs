using TerryU16.AtCoderHelper.CodeFormatters;
using TerryU16.AtCoderHelper.TestCases;

namespace TerryU16.AtCoderHelper;

internal class Command : ConsoleAppBase
{
    [Command("test", "テストケースを取得します。")]
    public async Task GetTestCases([Option("c", "コンテスト名")] string contestName, [Option("p", "問題名")] string problemName)
    {
        var testCaseManager = new TestCaseManager();
        await testCaseManager.GetTestCaseAsync(contestName, problemName, Context.CancellationToken);
    }

    [Command("format", "提出用コードを整形します。")]
    public async Task FormatCode([Option("p", "問題名")] string problemName)
    {
        var formatter = new CodeFormatter();
        await formatter.ConcatAsync(problemName, Context.CancellationToken);
    }
}
