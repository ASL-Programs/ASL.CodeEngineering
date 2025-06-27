using ASL.CodeEngineering.AI;
using Xunit;

namespace ASL.CodeEngineering.Tests;

public class FeatureLanguageAnalyzerTests
{
    [Theory]
    [InlineData("Add statistical charts", "R")]
    [InlineData("Create automation script", "Python")]
    [InlineData("High performance compute module", "C++")]
    [InlineData("Web frontend component", "Node.js")]
    [InlineData("Extend the WPF UI", "C#")]
    public void Recommend_ReturnsExpectedLanguage(string description, string expected)
    {
        string result = FeatureLanguageAnalyzer.Recommend(description);
        Assert.Equal(expected, result);
    }
}
