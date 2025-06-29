using System.Globalization;
using System.Resources;

namespace ASL.CodeEngineering;

public static class Strings
{
    private static readonly ResourceManager _rm = new(
        "ASL.CodeEngineering.App.Resources.Strings", typeof(Strings).Assembly);

    public static CultureInfo? Culture { get; set; }

    public static string AppTitle => _rm.GetString(nameof(AppTitle), Culture) ?? string.Empty;
    public static string OpenProject => _rm.GetString(nameof(OpenProject), Culture) ?? string.Empty;
    public static string Analyze => _rm.GetString(nameof(Analyze), Culture) ?? string.Empty;
    public static string Run => _rm.GetString(nameof(Run), Culture) ?? string.Empty;
    public static string Build => _rm.GetString(nameof(Build), Culture) ?? string.Empty;
    public static string Test => _rm.GetString(nameof(Test), Culture) ?? string.Empty;
    public static string PreviewUpdate => _rm.GetString(nameof(PreviewUpdate), Culture) ?? string.Empty;
    public static string Send => _rm.GetString(nameof(Send), Culture) ?? string.Empty;
    public static string StartLearning => _rm.GetString(nameof(StartLearning), Culture) ?? string.Empty;
    public static string Pause => _rm.GetString(nameof(Pause), Culture) ?? string.Empty;
    public static string Resume => _rm.GetString(nameof(Resume), Culture) ?? string.Empty;
    public static string Dashboard => _rm.GetString(nameof(Dashboard), Culture) ?? string.Empty;
    public static string Accept => _rm.GetString(nameof(Accept), Culture) ?? string.Empty;
    public static string Rollback => _rm.GetString(nameof(Rollback), Culture) ?? string.Empty;
    public static string LearningOn => _rm.GetString(nameof(LearningOn), Culture) ?? string.Empty;
    public static string StartNewProject => _rm.GetString(nameof(StartNewProject), Culture) ?? string.Empty;
    public static string Stop => _rm.GetString(nameof(Stop), Culture) ?? string.Empty;
    public static string MainTab => _rm.GetString(nameof(MainTab), Culture) ?? string.Empty;
    public static string PluginsTab => _rm.GetString(nameof(PluginsTab), Culture) ?? string.Empty;
    public static string ProvidersHeader => _rm.GetString(nameof(ProvidersHeader), Culture) ?? string.Empty;
    public static string AnalyzersHeader => _rm.GetString(nameof(AnalyzersHeader), Culture) ?? string.Empty;
    public static string RunnersHeader => _rm.GetString(nameof(RunnersHeader), Culture) ?? string.Empty;
    public static string BuildTestRunnersHeader => _rm.GetString(nameof(BuildTestRunnersHeader), Culture) ?? string.Empty;
    public static string EnabledColumn => _rm.GetString(nameof(EnabledColumn), Culture) ?? string.Empty;
    public static string NameColumn => _rm.GetString(nameof(NameColumn), Culture) ?? string.Empty;
    public static string VersionColumn => _rm.GetString(nameof(VersionColumn), Culture) ?? string.Empty;
    public static string TimeHeader => _rm.GetString(nameof(TimeHeader), Culture) ?? string.Empty;
    public static string ProviderHeader => _rm.GetString(nameof(ProviderHeader), Culture) ?? string.Empty;
    public static string SuggestionHeader => _rm.GetString(nameof(SuggestionHeader), Culture) ?? string.Empty;
    public static string DuplicateProvider => _rm.GetString(nameof(DuplicateProvider), Culture) ?? string.Empty;
    public static string DuplicateAnalyzer => _rm.GetString(nameof(DuplicateAnalyzer), Culture) ?? string.Empty;
    public static string DuplicateRunner => _rm.GetString(nameof(DuplicateRunner), Culture) ?? string.Empty;
    public static string DuplicateBuildTestRunner => _rm.GetString(nameof(DuplicateBuildTestRunner), Culture) ?? string.Empty;
    public static string Sending => _rm.GetString(nameof(Sending), Culture) ?? string.Empty;
    public static string Done => _rm.GetString(nameof(Done), Culture) ?? string.Empty;
    public static string Error => _rm.GetString(nameof(Error), Culture) ?? string.Empty;
    public static string LogError => _rm.GetString(nameof(LogError), Culture) ?? string.Empty;
    public static string SummaryError => _rm.GetString(nameof(SummaryError), Culture) ?? string.Empty;
    public static string Analyzing => _rm.GetString(nameof(Analyzing), Culture) ?? string.Empty;
    public static string Running => _rm.GetString(nameof(Running), Culture) ?? string.Empty;
    public static string Building => _rm.GetString(nameof(Building), Culture) ?? string.Empty;
    public static string Testing => _rm.GetString(nameof(Testing), Culture) ?? string.Empty;
    public static string Learning => _rm.GetString(nameof(Learning), Culture) ?? string.Empty;
    public static string Paused => _rm.GetString(nameof(Paused), Culture) ?? string.Empty;
    public static string ApplyUpdateQuestion => _rm.GetString(nameof(ApplyUpdateQuestion), Culture) ?? string.Empty;
    public static string PreviewCaption => _rm.GetString(nameof(PreviewCaption), Culture) ?? string.Empty;
    public static string Cancelled => _rm.GetString(nameof(Cancelled), Culture) ?? string.Empty;
    public static string DashboardTitle => _rm.GetString(nameof(DashboardTitle), Culture) ?? string.Empty;
}
