using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

public record CrawlResult(string Url, List<string> Snippets);

/// <summary>
/// Simple crawler that fetches documentation pages and extracts code snippets.
/// Results are appended to knowledge_base/meta/crawl.jsonl and
/// knowledge_base/meta/crawl_summary.md.
/// </summary>
public static class DocumentationCrawler
{
    private static readonly Regex CodeRegex = new("```(?<code>[^`]*)```", RegexOptions.Singleline | RegexOptions.Compiled);

    public static async Task<List<CrawlResult>> CrawlAsync(IEnumerable<string> urls, string projectRoot, CancellationToken token = default)
    {
        var results = new List<CrawlResult>();
        using var http = new HttpClient();
        foreach (var url in urls)
        {
            token.ThrowIfCancellationRequested();
            string content = url.StartsWith("http", StringComparison.OrdinalIgnoreCase)
                ? await http.GetStringAsync(url, token)
                : await File.ReadAllTextAsync(url, token);
            var snippets = new List<string>();
            foreach (Match m in CodeRegex.Matches(content))
            {
                string code = m.Groups["code"].Value.Trim();
                if (!string.IsNullOrWhiteSpace(code))
                    snippets.Add(code);
            }
            results.Add(new CrawlResult(url, snippets));
        }

        string metaDir = Path.Combine(projectRoot, "knowledge_base", "meta");
        Directory.CreateDirectory(metaDir);
        string jsonlPath = Path.Combine(metaDir, "crawl.jsonl");
        foreach (var r in results)
        {
            string line = JsonSerializer.Serialize(r);
            await File.AppendAllTextAsync(jsonlPath, line + Environment.NewLine, token);
        }

        string mdPath = Path.Combine(metaDir, "crawl_summary.md");
        using var writer = new StreamWriter(mdPath, append: true);
        foreach (var r in results)
        {
            await writer.WriteLineAsync($"## {r.Url}");
            foreach (var s in r.Snippets)
            {
                await writer.WriteLineAsync("```");
                await writer.WriteLineAsync(s);
                await writer.WriteLineAsync("```");
            }
        }

        return results;
    }
}
