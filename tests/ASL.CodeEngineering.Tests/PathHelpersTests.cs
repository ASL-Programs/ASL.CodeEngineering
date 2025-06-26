using System.IO;
using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class PathHelpersTests
{
    [Theory]
    [InlineData("ValidName", "ValidName")]
    [InlineData("Bad:Name", "Bad_Name")]
    [InlineData("More?Bad*Chars", "More_Bad_Chars")]
    public void SanitizeFileName_ReplacesInvalidChars(string input, string expected)
    {
        string result = PathHelpers.SanitizeFileName(input);
        Assert.Equal(expected, result);
        foreach (char c in Path.GetInvalidFileNameChars())
            Assert.DoesNotContain(c.ToString(), result);
    }
}
