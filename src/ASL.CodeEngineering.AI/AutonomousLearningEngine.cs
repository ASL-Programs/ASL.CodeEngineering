using System;
using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using ASL.CodeEngineering.AI.OfflineLearning;

namespace ASL.CodeEngineering.AI;

/// <summary>
/// Background service that performs autonomous learning cycles using the
/// currently selected <see cref="IAIProvider"/>. Each cycle requests
/// self-improvement suggestions and appends them to
/// <c>knowledge_base/auto/auto.jsonl</c>.
/// </summary>
public static class AutonomousLearningEngine
{
    public static async Task RunAsync(Func<IAIProvider> getProvider, CancellationToken token,
                                      IEnumerable<string>? packages = null,
                                      OfflineLearning.OfflineModel? model = null,
                                      string? modelPath = null)
    {
        string baseKb = Environment.GetEnvironmentVariable("KB_DIR") ??
                         Path.Combine(AppContext.BaseDirectory, "knowledge_base");
        string autoDir = Path.Combine(baseKb, "auto");
        Directory.CreateDirectory(autoDir);
        string logPath = Path.Combine(autoDir, "auto.jsonl");

        string packageText = LoadPackages(baseKb, packages);

        int counter = 0;
        while (!token.IsCancellationRequested)
        {
            var provider = getProvider();
            string result;
            try
            {
                string prompt = "Suggest an improvement to this autonomous code engineer and cite any useful online resources.";
                if (!string.IsNullOrWhiteSpace(packageText))
                {
                    prompt += "\n\n" + packageText;
                }
                result = await provider.SendChatAsync(prompt, token);
            }
            catch (Exception ex)
            {
                result = $"error: {ex.Message}";
            }

            var entry = new
            {
                timestamp = DateTime.UtcNow,
                provider = provider.Name,
                result
            };
            string line = JsonSerializer.Serialize(entry);
            try
            {
                File.AppendAllText(logPath, line + Environment.NewLine);
            }
            catch
            {
                // ignore log failures
            }

            if (model != null)
            {
                try
                {
                    var inputs = new List<double[]> { new[] { (double)result.Length } };
                    var targets = new List<double> { result.Length };
                    OfflineLearning.GradientTrainer.Train(model, inputs, targets, 0.001, 1);
                    if (modelPath != null)
                    {
                        OfflineLearning.ModelLoader.SavePt(model, modelPath);
                        OfflineLearning.ModelVersionManager.SaveVersion(AppContext.BaseDirectory, modelPath);
                    }
                }
                catch
                {
                    // ignore training failures
                }
            }

            try
            {
                await Task.Delay(TimeSpan.FromSeconds(5), token);
            }
            catch (TaskCanceledException)
            {
                break;
            }

            counter++;
            if (counter % 5 == 0)
            {
                try
                {
                    MetaAnalyzer.Analyze(AppContext.BaseDirectory);
                }
                catch
                {
                    // ignore analysis failures
                }
            }
        }
    }

    private static string LoadPackages(string baseKb, IEnumerable<string>? packages)
    {
        try
        {
            string dir = Path.Combine(baseKb, "packages");
            if (!Directory.Exists(dir))
                return string.Empty;
            IEnumerable<string> dirs = packages == null || !packages.Any()
                ? Directory.GetDirectories(dir).Select(Path.GetFileName)!
                : packages;
            var contents = new List<string>();
            foreach (var pkg in dirs)
            {
                string pkgDir = Path.Combine(dir, pkg);
                if (!Directory.Exists(pkgDir))
                    continue;
                foreach (var f in Directory.GetFiles(pkgDir, "*", SearchOption.AllDirectories))
                {
                    try
                    {
                        contents.Add(File.ReadAllText(f));
                    }
                    catch
                    {
                    }
                }
            }
            return string.Join("\n\n", contents);
        }
        catch
        {
            return string.Empty;
        }
    }
}
