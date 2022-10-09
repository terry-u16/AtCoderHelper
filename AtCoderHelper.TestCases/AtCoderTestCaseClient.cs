using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace TerryU16.AtCoderHelper.TestCases;

internal class AtCoderTestCaseClient
{
    readonly static Regex _inputCaseRegex = new(@"<h3>入力例\s?\d</h3>");
    readonly static Regex _outputCaseRegex = new(@"<h3>出力例\s?\d</h3>");

    private readonly HttpClient _client;

    public bool LoggedIn { get; private set; }

    public AtCoderTestCaseClient()
    {
        _client = new HttpClient
        {
            BaseAddress = new Uri("https://atcoder.jp")
        };
        LoggedIn = false;
    }

    public async Task<bool> LoginAsync(LoginCredential credential)
    {
        var loginUri = new Uri("login", UriKind.Relative);
        using var loginFormResult = await _client.GetAsync(loginUri);

        if (loginFormResult.IsSuccessStatusCode)
        {
            using var stream = await loginFormResult.Content.ReadAsStreamAsync();
            var parser = new HtmlParser();
            var html = await parser.ParseDocumentAsync(stream);
            var csrfTokenInput = html.QuerySelectorAll("input").First(e => e.Attributes["type"]?.Value == "hidden" && e.Attributes["name"]?.Value == "csrf_token");
            var csrfToken = WebUtility.HtmlDecode(csrfTokenInput.Attributes["value"]?.Value);

            var loginContent = new FormUrlEncodedContent(new Dictionary<string, string>()
                    {
                        { "username", credential.UserName ?? "" },
                        { "password", credential.Password ?? "" },
                        { "csrf_token", csrfToken ?? "" }
                    });

            var loginResult = await _client.PostAsync(loginUri, loginContent);
            LoggedIn = loginResult.IsSuccessStatusCode && loginResult.RequestMessage?.RequestUri?.AbsoluteUri == "https://atcoder.jp/home";
            return LoggedIn;
        }
        else
        {
            throw new NetworkConnectionException($"login uriが見付かりませんでした。");
        }
    }

    public async Task<TestCase[]> GetTestCasesAsync(string contestName, string problemName)
    {
        contestName = contestName.ToLower();
        problemName = problemName.ToLower();
        var uri = await GetQuestionUriAsync(contestName, problemName);

        using var result = await _client.GetAsync(uri);

        if (result.IsSuccessStatusCode)
        {
            var testCases = new List<TestCase>();
            using var stream = await result.Content.ReadAsStreamAsync();
            var parser = new HtmlParser();
            var doc = await parser.ParseDocumentAsync(stream);

            var inputTexts = doc.QuerySelectorAll("div.part").Where(e => _inputCaseRegex.IsMatch(e.InnerHtml)).Select(e => e.QuerySelector("pre")?.TextContent);
            var outputTexts = doc.QuerySelectorAll("div.part").Where(e => _outputCaseRegex.IsMatch(e.InnerHtml)).Select(e => e.QuerySelector("pre")?.TextContent);

            return inputTexts.Zip(outputTexts, (input, output) => new TestCase(input ?? "", output ?? "")).ToArray();
        }
        else
        {
            throw new NetworkConnectionException($"{(int)result.StatusCode} {result.ReasonPhrase}");
        }
    }

    private async Task<Uri> GetQuestionUriAsync(string contestName, string questionName)
    {
        var endPoint = new Uri($"contests/{contestName}/tasks", UriKind.Relative);
        using var result = await _client.GetAsync(endPoint);

        if (result.IsSuccessStatusCode)
        {
            using var stream = await result.Content.ReadAsStreamAsync();
            var parser = new HtmlParser();
            var html = await parser.ParseDocumentAsync(stream);

            var table = html.QuerySelector("table");
            var rows = table?.QuerySelectorAll("tr");
            var questions = rows?.Select(e => e.FirstElementChild?.FirstElementChild)?.Select(e => new { Uri = e?.GetAttribute("href"), Symbol = e?.TextContent });
            var matchedQuestion = questions?.FirstOrDefault(q => q?.Symbol?.Equals(questionName, StringComparison.OrdinalIgnoreCase) ?? false);

            if (matchedQuestion != null && matchedQuestion.Uri != null)
            {
                return new Uri(matchedQuestion.Uri, UriKind.Relative);
            }
            else
            {
                throw new NetworkConnectionException($"{contestName} - {questionName}が見付かりませんでした。");
            }
        }
        else
        {
            throw new NetworkConnectionException($"{(int)result.StatusCode} {result.ReasonPhrase}");
        }
    }
}
