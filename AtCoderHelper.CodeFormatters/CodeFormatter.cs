﻿using System.Text;
using System.Text.RegularExpressions;
using TextCopy;

namespace AtCoderHelper.CodeFormatters;

public class CodeFormatter
{
    private const string ProgramFileName = "Program.cs";
    private const string SolverDirectoryName = "Problems";
    private readonly static Regex _usingRegex = new(@"^using \S+?;$");

    public async Task ConcatAsync(string problemName)
    {
        var solverName = $"Problem{problemName.ToUpper()}";
        var solverFileName = $"{solverName}.cs";
        var solverPath = Path.Combine(Environment.CurrentDirectory, SolverDirectoryName, solverFileName);
        var programFilePath = Path.Combine(Environment.CurrentDirectory, ProgramFileName);

        if (!File.Exists(solverPath))
        {
            WriteLineWithColor($"{solverFileName} not found.", ConsoleColor.Red);
            await BeepAsync();
            return;
        }

        if (!File.Exists(programFilePath))
        {
            WriteLineWithColor($"{ProgramFileName} not found.", ConsoleColor.Red);
            await BeepAsync();
            return;
        }

        var output = new StringBuilder(await File.ReadAllTextAsync(solverPath));
        var programFileContents = await File.ReadAllLinesAsync(programFilePath);
        var solverConstructor = $"{solverName}();";

        if (!programFileContents.Any(line => line.Contains(solverConstructor)))
        {
            WriteLineWithColor($"{solverName} not found in {ProgramFileName}.", ConsoleColor.Red);
            await BeepAsync();
            return;
        }

        if (programFileContents.Any(line => line.Contains("new FileStream")))
        {
            WriteLineWithColor($"Warning! FileStream was detected.", ConsoleColor.Yellow);
            await BeepAsync();
        }

        foreach (var line in programFileContents.Where(line => !_usingRegex.IsMatch(line)))
        {
            output.AppendLine(line);
        }

        await ClipboardService.SetTextAsync(output.ToString());
        WriteLineWithColor($"{solverName} was copied to clipboard.", ConsoleColor.Cyan);
    }

    private static void WriteLineWithColor(string value, ConsoleColor color)
    {
        try
        {
            Console.ForegroundColor = color;
            Console.WriteLine(value);
        }
        finally
        {
            Console.ResetColor();
        }
    }

    private static async Task BeepAsync()
    {
        if (!OperatingSystem.IsWindows())
        {
            return;
        }

        for (int i = 0; i < 3; i++)
        {
            Console.Beep(800, 400);
            await Task.Delay(100);
        }
    }
}