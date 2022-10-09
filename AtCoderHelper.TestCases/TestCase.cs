namespace TerryU16.AtCoderHelper.TestCases;

public class TestCase : IEquatable<TestCase>
{
    public string Input { get; }
    public string Output { get; }

    public TestCase(string input, string output)
    {
        Input = input.Trim();
        Output = output.Trim();
    }

    public override string ToString() => $"[Input]{Environment.NewLine}{Input}{Environment.NewLine}[Output]{Environment.NewLine}{Output}";

    public bool Equals(TestCase? other) => Input == other?.Input && Output == other?.Output;

    public override bool Equals(object? obj) => Equals(obj as TestCase);

    public override int GetHashCode() => HashCode.Combine(Input, Output);
}