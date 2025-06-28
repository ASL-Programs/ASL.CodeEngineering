using System;
using System.IO;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class DocumentationCrawlerTests
{
    [Fact]
    public async Task CrawlAsync_WritesOutputFiles()
    {
        var temp = Directory.CreateTempSubdirectory();
        try
        {
            string htmlPath = Path.Combine(temp.FullName, "doc.html");
            await File.WriteAllTextAsync(htmlPath, "<pre><code>Console.WriteLine(\"hi\");</code></pre>");

            string projectRoot = temp.FullName;
            await DocumentationCrawler.CrawlAsync(new[] { htmlPath }, projectRoot);

            string metaDir = Path.Combine(projectRoot, "knowledge_base", "meta");
            Assert.True(File.Exists(Path.Combine(metaDir, "crawl.jsonl")));
            Assert.True(File.Exists(Path.Combine(metaDir, "crawl_summary.md")));
        }
        finally
        {
            Directory.Delete(temp.FullName, true);
        }
    }
}
