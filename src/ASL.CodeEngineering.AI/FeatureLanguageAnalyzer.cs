using System;

namespace ASL.CodeEngineering.AI;

public static class FeatureLanguageAnalyzer
{
    public static string Recommend(string description)
    {
        if (string.IsNullOrWhiteSpace(description))
            return "C#";

        string text = description.ToLowerInvariant();
        if (text.Contains("javascript") || text.Contains("node") || text.Contains("frontend"))
            return "Node.js";
        if (text.Contains("data") || text.Contains("statistics") || text.Contains("analysis"))
            return "R";
        if (text.Contains("machine learning") || text.Contains("automation") || text.Contains("script"))
            return "Python";
        if (text.Contains("performance") || text.Contains("system"))
            return "C++";
        if (text.Contains("ui") || text.Contains("wpf") || text.Contains("plugin"))
            return "C#";
        return "C#";
    }
}
