using System;
using System.IO;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class DocsUpdaterTests
{
    [Fact]
    public void RecordCycle_ArchivesAndUpdatesDocs()
    {
        var temp = Directory.CreateTempSubdirectory();
        try
        {
            string agentsPath = Path.Combine(temp.FullName, "AGENTS.md");
            string nextPath = Path.Combine(temp.FullName, "NEXT_STEPS.md");
            File.WriteAllText(agentsPath, "- [ ] TestTask\n");
            File.WriteAllText(nextPath, "- [ ] TestTask\n");

            DocsUpdater.RecordCycle(temp.FullName, new[] { "TestTask" }, 1);

            string archiveDir = Path.Combine(temp.FullName, "docs", "archive");
            Assert.True(Directory.Exists(archiveDir));
            Assert.Contains(Directory.GetFiles(archiveDir), f => f.Contains("AGENTS_"));
            Assert.Contains(Directory.GetFiles(archiveDir), f => f.Contains("NEXT_STEPS_"));

            string[] agentsLines = File.ReadAllLines(agentsPath);
            Assert.Contains(agentsLines, l => l.StartsWith("- [x] TestTask"));
            Assert.Contains(agentsLines, l => l.Contains("Learning cycle 1 completed"));

            string[] nextLines = File.ReadAllLines(nextPath);
            Assert.Contains(nextLines, l => l.StartsWith("- [x] TestTask"));
        }
        finally
        {
            Directory.Delete(temp.FullName, true);
        }
    }
}
