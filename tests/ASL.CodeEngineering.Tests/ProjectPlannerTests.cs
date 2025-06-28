using System.IO;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class ProjectPlannerTests
{
    [Fact]
    public void GeneratePlans_WritesPlanFile()
    {
        var temp = Directory.CreateTempSubdirectory();
        try
        {
            File.WriteAllText(Path.Combine(temp.FullName, "AGENTS.md"), "- [ ] Implement module\n");
            string src = Path.Combine(temp.FullName, "src");
            Directory.CreateDirectory(Path.Combine(src, "Module"));

            var plans = ProjectPlanner.GeneratePlans(temp.FullName);
            string planPath = Path.Combine(temp.FullName, "knowledge_base", "plans", "plans.json");
            Assert.True(File.Exists(planPath));
            Assert.Contains(plans, p => p.Name == "Module");
        }
        finally
        {
            Directory.Delete(temp.FullName, true);
        }
    }
}
