using System.IO;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class ProjectGeneratorTests
{
    [Fact]
    public async Task GenerateAsync_CreatesProjectFiles()
    {
        var temp = Directory.CreateTempSubdirectory();
        try
        {
            string path = await ProjectGenerator.GenerateAsync("My Project", "C#", temp.FullName);
            Assert.True(Directory.Exists(path));
            Assert.True(File.Exists(Path.Combine(path, "README.md")));
            Assert.True(File.Exists(Path.Combine(path, "src", "My_Project.csproj")));
        }
        finally
        {
            Directory.Delete(temp.FullName, true);
        }
    }
}
