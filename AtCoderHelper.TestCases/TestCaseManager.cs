using TextCopy;

namespace TerryU16.AtCoderHelper.TestCases;

internal class TestCaseManager
{
    private readonly string _credentialPath;
    private readonly AtCoderTestCaseClient _client;
    private readonly CredentialManager _credentialManager;

    public TestCaseManager()
    {
        _credentialPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "credentials.bin");
        _client = new AtCoderTestCaseClient();
        _credentialManager = new CredentialManager();
    }

    public async Task GetTestCaseAsync(string contestName, string problemName, CancellationToken ct = default)
    {
        if (!_client.LoggedIn)
        {
            await LoginAsync(ct);
        }

        await GetTestCaseInnerAsync(contestName, problemName, ct);
    }

    private async Task LoginAsync(CancellationToken ct = default)
    {
        var credential = await TryGetCredentialAsync(ct);

        if (credential is not null && await _client.LoginAsync(credential, ct))
        {
            Console.WriteLine($"You're logged in as {credential.UserName}");
            return;
        }

        while (true)
        {
            Console.WriteLine("Input your credential.");
            Console.Write("username: ");
            var userName = Console.ReadLine() ?? throw new InvalidOperationException();
            Console.Write("password: ");
            var password = Console.ReadLine() ?? throw new InvalidOperationException();
            credential = new LoginCredential(userName, password);

            if (await _client.LoginAsync(credential, ct))
            {
                Console.WriteLine($"You're logged in as {credential.UserName}");
                using var stream = new FileStream(_credentialPath, FileMode.Create, FileAccess.Write);
                await _credentialManager.SaveCredentialAsync(stream, credential, ct);
                return;
            }

            Console.WriteLine("Login failed.");
        }
    }

    private async Task<LoginCredential?> TryGetCredentialAsync(CancellationToken ct = default)
    {
        if (File.Exists(_credentialPath))
        {
            return null;
        }

        try
        {
            using var stream = new FileStream(_credentialPath, FileMode.Open, FileAccess.Read);
            var credential = await _credentialManager.LoadCredendialAsync(stream, ct);
            return credential;
        }
        catch (Exception)
        {
            return null;
        }
    }

    private async Task GetTestCaseInnerAsync(string contestName, string problemName, CancellationToken ct = default)
    {
        try
        {
            var testCases = await _client.GetTestCasesAsync(contestName, problemName, ct);

            if (testCases.Length == 0)
            {
                Console.WriteLine("Failed to get test cases.");
                return;
            }

            foreach (var testCase in testCases)
            {
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("[Input]");
                Console.ForegroundColor = ConsoleColor.DarkGreen;
                Console.WriteLine($"{testCase.Input}");
                Console.ForegroundColor = ConsoleColor.DarkGray;
                Console.WriteLine("[Output]");
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"{testCase.Output}");
            }

            await CopyToClipboardAsync(testCases, ct);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("=> Copied to clipboard.");
            Console.ResetColor();
        }
        catch (Exception)
        {
            Console.WriteLine("Failed to get test cases.");
        }
    }

    private static async Task CopyToClipboardAsync(TestCase[] testCases, CancellationToken ct = default)
    {
        var text = string.Join(Environment.NewLine, testCases.Select(t => $"[InlineData(@\"{t.Input}\", @\"{t.Output}\")]"));
        await ClipboardService.SetTextAsync($"[Theory]{Environment.NewLine}{text}", ct);
    }
}
