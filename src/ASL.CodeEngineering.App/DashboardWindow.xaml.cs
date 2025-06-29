using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Windows;
using ASL.CodeEngineering.AI;

namespace ASL.CodeEngineering;

public partial class DashboardWindow : Window
{
    public DashboardWindow(string projectRoot)
    {
        InitializeComponent();
        LoadData(projectRoot);
    }

    private void LoadData(string root)
    {
        string metaDir = Path.Combine(root, "knowledge_base", "meta");
        string crawlFile = Path.Combine(metaDir, "crawl.jsonl");
        string planFile = Path.Combine(root, "knowledge_base", "plans", "plans.json");
        string insightsFile = Path.Combine(metaDir, "language_insights.json");

        var plans = new List<ModulePlan>();
        if (File.Exists(planFile))
        {
            try
            {
                plans = JsonSerializer.Deserialize<List<ModulePlan>>(File.ReadAllText(planFile)) ?? new();
            }
            catch
            {
                // ignore parse errors
            }
        }
        PlanGrid.ItemsSource = plans;

        var crawlRows = new List<dynamic>();
        if (File.Exists(crawlFile))
        {
            foreach (var line in File.ReadAllLines(crawlFile))
            {
                try
                {
                    var data = JsonSerializer.Deserialize<CrawlResult>(line);
                    crawlRows.Add(new { data!.Url, Snippets = data.Snippets.Count });
                }
                catch
                {
                    // ignore
                }
            }
        }
        SummaryGrid.ItemsSource = crawlRows;

        var insights = new List<dynamic>();
        if (File.Exists(insightsFile))
        {
            try
            {
                insights = JsonSerializer.Deserialize<List<dynamic>>(File.ReadAllText(insightsFile)) ?? new();
            }
            catch
            {
                // ignore
            }
        }
        InsightGrid.ItemsSource = insights;
    }
}
