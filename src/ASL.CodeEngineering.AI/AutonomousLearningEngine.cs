using System;
using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace ASL.CodeEngineering.AI;

/// <summary>
/// Background service that performs autonomous learning cycles using the
/// currently selected <see cref="IAIProvider"/>. Each cycle requests
/// self-improvement suggestions and appends them to
/// <c>knowledge_base/auto/auto.jsonl</c>.
/// </summary>
public static class AutonomousLearningEngine
{
    public static async Task RunAsync(Func<IAIProvider> getProvider, CancellationToken token)
    {
        string baseKb = Environment.GetEnvironmentVariable("KB_DIR") ??
                         Path.Combine(AppContext.BaseDirectory, "knowledge_base");
        string autoDir = Path.Combine(baseKb, "auto");
        Directory.CreateDirectory(autoDir);
        string logPath = Path.Combine(autoDir, "auto.jsonl");

        int counter = 0;
        while (!token.IsCancellationRequested)
        {
            var provider = getProvider();
            string result;
            try
            {
                string prompt = "Suggest an improvement to this autonomous code engineer and cite any useful online resources.";
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
}
