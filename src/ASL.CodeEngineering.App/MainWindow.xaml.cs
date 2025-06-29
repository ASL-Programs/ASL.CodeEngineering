using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text.Json;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using ICSharpCode.AvalonEdit.Highlighting;
using Forms = System.Windows.Forms;
using ASL.CodeEngineering.AI;

namespace ASL.CodeEngineering
{
    public partial class MainWindow : Window
    {

        private readonly Dictionary<string, Func<IAIProvider>> _providerFactories = new();
        private readonly Dictionary<string, Func<IAnalyzerPlugin>> _analyzerFactories = new();
        private readonly Dictionary<string, Func<ICodeRunnerPlugin>> _runnerFactories = new();
        private readonly Dictionary<string, Func<IBuildTestRunner>> _buildTestRunnerFactories = new();
        private IAIProvider _aiProvider = new EchoAIProvider();
        private IAnalyzerPlugin? _analyzer;
        private ICodeRunnerPlugin? _runner;
        private IBuildTestRunner? _buildTestRunner;
        private string _projectRoot = AppContext.BaseDirectory;
        private CancellationTokenSource? _learningCts;
        private Task? _learningTask;
        private CancellationTokenSource? _monitorCts;
        private Task? _monitorTask;
        private readonly ObservableCollection<LearningEntry> _learningEntries = new();
        private readonly string _learningStatePath;
        private readonly ObservableCollection<string> _packageNames = new();
        private readonly Dictionary<string, bool> _selectedPackages = new();
        private string? _latestVersionPath;
        private CancellationTokenSource? _projectCts;
        private Task? _projectTask;


        public MainWindow()
        {
            InitializeComponent();

            LearningGrid.ItemsSource = _learningEntries;
            _learningStatePath = Path.Combine(AppContext.BaseDirectory, "data", "learning_enabled.txt");

            string baseKb = Environment.GetEnvironmentVariable("KB_DIR") ??
                               Path.Combine(AppContext.BaseDirectory, "knowledge_base");
            string pkgDir = Path.Combine(baseKb, "packages");
            if (Directory.Exists(pkgDir))
            {
                foreach (var dir in Directory.GetDirectories(pkgDir))
                {
                    string name = Path.GetFileName(dir);
                    _packageNames.Add(name);
                    _selectedPackages[name] = true;
                }
            }
            PackageList.ItemsSource = _packageNames;

            _providerFactories["Echo"] = () => new EchoAIProvider();
            _providerFactories["Reverse"] = () => new ReverseAIProvider();
            _providerFactories["Local"] = () => new LocalAIProvider();
            _providerFactories["OpenAI"] = () => new OpenAIProvider();

            _analyzerFactories["Todo"] = () => new TodoAnalyzer();
            _runnerFactories["DotnetVersion"] = () => new DotnetVersionRunner();
            _buildTestRunnerFactories["Dotnet"] = () => new DotnetBuildTestRunner();
            _buildTestRunnerFactories["Python"] = () => new PythonBuildTestRunner();

            foreach (var pair in AIProviderLoader.LoadProviders(AppContext.BaseDirectory))
            {
                if (_providerFactories.ContainsKey(pair.Key))
                {
                    StatusTextBlock.Text = $"Duplicate provider: {pair.Key}";
                    LogError("DuplicateProvider", new InvalidOperationException($"Duplicate provider name '{pair.Key}'"));
                }
                else
                {
                    _providerFactories[pair.Key] = pair.Value;
                }
            }

            foreach (var pair in PluginLoader.LoadAnalyzers(AppContext.BaseDirectory))
            {
                if (_analyzerFactories.ContainsKey(pair.Key))
                {
                    StatusTextBlock.Text = $"Duplicate analyzer: {pair.Key}";
                    LogError("DuplicateAnalyzer", new InvalidOperationException($"Duplicate analyzer name '{pair.Key}'"));
                }
                else
                {
                    _analyzerFactories[pair.Key] = pair.Value;
                }
            }

            foreach (var pair in PluginLoader.LoadRunners(AppContext.BaseDirectory))
            {
                if (_runnerFactories.ContainsKey(pair.Key))
                {
                    StatusTextBlock.Text = $"Duplicate runner: {pair.Key}";
                    LogError("DuplicateRunner", new InvalidOperationException($"Duplicate runner name '{pair.Key}'"));
                }
                else
                {
                    _runnerFactories[pair.Key] = pair.Value;
                }
            }

            foreach (var pair in PluginLoader.LoadBuildTestRunners(AppContext.BaseDirectory))
            {
                if (_buildTestRunnerFactories.ContainsKey(pair.Key))
                {
                    StatusTextBlock.Text = $"Duplicate build/test runner: {pair.Key}";
                    LogError("DuplicateBuildTestRunner", new InvalidOperationException($"Duplicate build/test runner name '{pair.Key}'"));
                }
                else
                {
                    _buildTestRunnerFactories[pair.Key] = pair.Value;
                }
            }

            string? disableNet = Environment.GetEnvironmentVariable("DISABLE_NETWORK_PROVIDERS");
            bool offline = !string.IsNullOrWhiteSpace(disableNet) &&
                           (disableNet == "1" || disableNet.Equals("true", StringComparison.OrdinalIgnoreCase));
            if (offline)
            {
                foreach (var key in _providerFactories.Keys.ToList())
                {
                    var provider = _providerFactories[key]();
                    if (provider.RequiresNetwork)
                        _providerFactories.Remove(key);
                }
            }

            ProviderComboBox.ItemsSource = _providerFactories.Keys;
            ProviderComboBox.SelectedIndex = 0;
            AnalyzerComboBox.ItemsSource = _analyzerFactories.Keys;
            AnalyzerComboBox.SelectedIndex = _analyzerFactories.Count > 0 ? 0 : -1;
            RunnerComboBox.ItemsSource = _runnerFactories.Keys;
            RunnerComboBox.SelectedIndex = _runnerFactories.Count > 0 ? 0 : -1;
            BuildTestRunnerComboBox.ItemsSource = _buildTestRunnerFactories.Keys;
            BuildTestRunnerComboBox.SelectedIndex = _buildTestRunnerFactories.Count > 0 ? 0 : -1;

            LoadProjectTree(_projectRoot);

            if (File.Exists(_learningStatePath) && File.ReadAllText(_learningStatePath).Trim() == "1")
            {
                LearningEnabledCheckBox.IsChecked = true;
                StartLearningLoop();
            }
        }

        private void ProviderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProviderComboBox.SelectedItem is string key &&
                _providerFactories.TryGetValue(key, out var factory))
            {
                _aiProvider = factory();
            }
        }

        private void AnalyzerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AnalyzerComboBox.SelectedItem is string key &&
                _analyzerFactories.TryGetValue(key, out var factory))
            {
                _analyzer = factory();
            }
        }

        private void RunnerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RunnerComboBox.SelectedItem is string key &&
                _runnerFactories.TryGetValue(key, out var factory))
            {
                _runner = factory();
            }
        }

        private void BuildTestRunnerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BuildTestRunnerComboBox.SelectedItem is string key &&
                _buildTestRunnerFactories.TryGetValue(key, out var factory))
            {
                _buildTestRunner = factory();
            }
        }


        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string prompt = PromptTextBox.Text;
            if (string.IsNullOrWhiteSpace(prompt))
                return;

            SendButton.IsEnabled = false;
            StatusTextBlock.Text = "Sending...";

            string response;
            try
            {
                response = await _aiProvider.SendChatAsync(prompt, CancellationToken.None);
                ResponseTextBox.Text = response;
                StatusTextBlock.Text = "Done";
            }
            catch (Exception ex)
            {
                response = ex.Message;
                ResponseTextBox.Text = ex.Message;
                StatusTextBlock.Text = "Error";
                LogError("SendChat", ex);
            }

            SendButton.IsEnabled = true;

            string providerName = PathHelpers.SanitizeFileName(_aiProvider.Name);
            string baseData = Environment.GetEnvironmentVariable("DATA_DIR") ??
                              Path.Combine(AppContext.BaseDirectory, "data");
            string dataDir = Path.Combine(baseData, providerName);
            Directory.CreateDirectory(dataDir);
            string chatPath = Path.Combine(dataDir, "chatlog.jsonl");
            var chatEntry = new { timestamp = DateTime.UtcNow, prompt, response };
            string chatLine = JsonSerializer.Serialize(chatEntry);
            try
            {
                File.AppendAllText(chatPath, chatLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                LogError("ChatLogWrite", ex);
                StatusTextBlock.Text = "Log Error";
            }

            // Generate a brief summary using the active provider and store it in the knowledge base
            string summary;
            try
            {
                string summaryPrompt = $"Summarize the following conversation in one sentence. Prompt: {prompt} Response: {response}";
                summary = await _aiProvider.SendChatAsync(summaryPrompt, CancellationToken.None);
            }
            catch (Exception ex)
            {
                summary = $"[error: {ex.Message}]";
                LogError("SummaryGeneration", ex);
                StatusTextBlock.Text = "Summary Error";
            }

            string baseKb = Environment.GetEnvironmentVariable("KB_DIR") ??
                            Path.Combine(AppContext.BaseDirectory, "knowledge_base");
            string knowledgeDir = Path.Combine(baseKb, providerName);
            Directory.CreateDirectory(knowledgeDir);
            string summaryPath = Path.Combine(knowledgeDir, "summaries.jsonl");
            var summaryEntry = new { timestamp = DateTime.UtcNow, summary };
            string summaryLine = JsonSerializer.Serialize(summaryEntry);

            string metaDir = Path.Combine(baseKb, "meta");
            Directory.CreateDirectory(metaDir);
            string metaPath = Path.Combine(metaDir, "summaries.jsonl");
            var metaEntry = new { timestamp = DateTime.UtcNow, provider = providerName, summary };
            string metaLine = JsonSerializer.Serialize(metaEntry);
            try
            {
                File.AppendAllText(summaryPath, summaryLine + Environment.NewLine);
                File.AppendAllText(metaPath, metaLine + Environment.NewLine);
            }
            catch (Exception ex)
            {
                LogError("SummaryWrite", ex);
                StatusTextBlock.Text = "Log Error";
            }
        }

        private async void AnalyzeButton_Click(object sender, RoutedEventArgs e)
        {
            if (_analyzer is null)
                return;
            string code = PromptTextBox.Text;
            if (string.IsNullOrWhiteSpace(code))
                return;

            AnalyzeButton.IsEnabled = false;
            StatusTextBlock.Text = "Analyzing...";
            try
            {
                var result = await _analyzer.AnalyzeAsync(code, CancellationToken.None);
                ResponseTextBox.Text = result;
                StatusTextBlock.Text = "Done";
            }
            catch (Exception ex)
            {
                ResponseTextBox.Text = ex.Message;
                StatusTextBlock.Text = "Error";
            }
            finally
            {
                AnalyzeButton.IsEnabled = true;
            }
        }

        private async void RunButton_Click(object sender, RoutedEventArgs e)
        {
            if (_runner is null)
                return;

            RunButton.IsEnabled = false;
            StatusTextBlock.Text = "Running...";
            try
            {
                var result = await _runner.RunAsync(_projectRoot, CancellationToken.None);
                ResponseTextBox.Text = result;
                StatusTextBlock.Text = "Done";
            }
            catch (Exception ex)
            {
                ResponseTextBox.Text = ex.Message;
                StatusTextBlock.Text = "Error";
            }
            finally
            {
                RunButton.IsEnabled = true;
            }
        }

        private async void BuildButton_Click(object sender, RoutedEventArgs e)
        {
            if (_buildTestRunner is null)
                return;

            BuildButton.IsEnabled = false;
            StatusTextBlock.Text = "Building...";
            try
            {
                _latestVersionPath = await BuildProcess.BuildNewVersionAsync(_projectRoot, _buildTestRunner, CancellationToken.None);
                ResponseTextBox.Text = _latestVersionPath;
                StatusTextBlock.Text = "Done";
            }
            catch (Exception ex)
            {
                ResponseTextBox.Text = ex.Message;
                StatusTextBlock.Text = "Error";
            }
            finally
            {
                BuildButton.IsEnabled = true;
            }
        }

        private async void TestButton_Click(object sender, RoutedEventArgs e)
        {
            if (_buildTestRunner is null)
                return;

            TestButton.IsEnabled = false;
            StatusTextBlock.Text = "Testing...";
            try
            {
                var result = await _buildTestRunner.TestAsync(_projectRoot, CancellationToken.None);
                ResponseTextBox.Text = result;
                StatusTextBlock.Text = "Done";
            }
            catch (Exception ex)
            {
                ResponseTextBox.Text = ex.Message;
                StatusTextBlock.Text = "Error";
            }
            finally
            {
                TestButton.IsEnabled = true;
            }
        }

        private void StartLearningButton_Click(object sender, RoutedEventArgs e)
        {
            StartLearningLoop();
        }

        private void PauseLearningButton_Click(object sender, RoutedEventArgs e)
        {
            PauseLearningLoop();
        }

        private void ResumeLearningButton_Click(object sender, RoutedEventArgs e)
        {
            StartLearningLoop();
        }

        private void DashboardButton_Click(object sender, RoutedEventArgs e)
        {
            var window = new DashboardWindow(_projectRoot);
            window.Owner = this;
            window.Show();
        }

        private void OpenProjectButton_Click(object sender, RoutedEventArgs e)
        {
            using var dialog = new Forms.FolderBrowserDialog();
            dialog.SelectedPath = _projectRoot;
            if (dialog.ShowDialog() == Forms.DialogResult.OK)
            {
                _projectRoot = dialog.SelectedPath;
                LoadProjectTree(_projectRoot);
            }
        }

        private void ProjectTreeView_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            if (e.NewValue is TreeViewItem item && File.Exists(item.Tag?.ToString()))
            {
                string path = item.Tag!.ToString()!;
                CodeEditor.Load(path);
                CodeEditor.SyntaxHighlighting = HighlightingManager.Instance.GetDefinitionByExtension(Path.GetExtension(path)) ?? HighlightingManager.Instance.GetDefinition("Text");
            }
        }

        private void LoadProjectTree(string root)
        {
            ProjectTreeView.Items.Clear();
            var rootItem = new TreeViewItem { Header = Path.GetFileName(root), Tag = root };
            LoadDirectory(rootItem, root);
            ProjectTreeView.Items.Add(rootItem);
            rootItem.IsExpanded = true;
        }

        private void LoadDirectory(TreeViewItem parentItem, string path)
        {
            foreach (var dir in Directory.GetDirectories(path).OrderBy(d => d, StringComparer.OrdinalIgnoreCase))
            {
                var item = new TreeViewItem { Header = Path.GetFileName(dir), Tag = dir };
                LoadDirectory(item, dir);
                parentItem.Items.Add(item);
            }

            foreach (var file in Directory.GetFiles(path).OrderBy(f => f, StringComparer.OrdinalIgnoreCase))
            {
                parentItem.Items.Add(new TreeViewItem { Header = Path.GetFileName(file), Tag = file });
            }
        }

        private void StartLearningLoop()
        {
            if (_learningTask is not null && !_learningTask.IsCompleted)
                return;

            _learningCts = new CancellationTokenSource();
            var packages = _selectedPackages.Where(p => p.Value).Select(p => p.Key).ToArray();
            _learningTask = Task.Run(() => AutonomousLearningEngine.RunAsync(() => _aiProvider, _learningCts.Token, packages));
            StartMonitor();
            StatusTextBlock.Text = "Learning...";
        }

        private void PauseLearningLoop()
        {
            _learningCts?.Cancel();
            _monitorCts?.Cancel();
            StatusTextBlock.Text = "Paused";
        }

        private void StartMonitor()
        {
            _monitorCts?.Cancel();
            _monitorCts = new CancellationTokenSource();
            string baseKb = Environment.GetEnvironmentVariable("KB_DIR") ?? Path.Combine(AppContext.BaseDirectory, "knowledge_base");
            string logPath = Path.Combine(baseKb, "auto", "auto.jsonl");
            _monitorTask = Task.Run(() => MonitorLogAsync(logPath, _monitorCts.Token));
        }

        private void PackageCheckBox_Changed(object sender, RoutedEventArgs e)
        {
            if (sender is CheckBox cb && cb.Content is string name)
            {
                _selectedPackages[name] = cb.IsChecked == true;
            }
        }

        private async Task MonitorLogAsync(string path, CancellationToken token)
        {
            while (!token.IsCancellationRequested)
            {
                try
                {
                    if (File.Exists(path))
                    {
                        var lines = File.ReadAllLines(path);
                        var items = new List<LearningEntry>();
                        foreach (var line in lines)
                        {
                            try
                            {
                                var entry = JsonSerializer.Deserialize<LearningEntry>(line);
                                if (entry != null)
                                    items.Add(entry);
                            }
                            catch { }
                        }
                        await Dispatcher.InvokeAsync(() =>
                        {
                            _learningEntries.Clear();
                            foreach (var item in items)
                                _learningEntries.Add(item);
                        });
                    }
                    await Task.Delay(2000, token);
                }
                catch (TaskCanceledException)
                {
                    break;
                }
                catch
                {
                }
            }
        }

        private void AcceptSuggestionButton_Click(object sender, RoutedEventArgs e)
        {
            if (LearningGrid.SelectedItem is LearningEntry entry)
            {
                _latestVersionPath = VersionManager.SaveVersion(_projectRoot);
                string metaDir = Path.Combine(AppContext.BaseDirectory, "knowledge_base", "meta");
                Directory.CreateDirectory(metaDir);
                string path = Path.Combine(metaDir, "accepted.jsonl");
                File.AppendAllText(path, JsonSerializer.Serialize(entry) + Environment.NewLine);
            }
        }

        private void RollbackSuggestionButton_Click(object sender, RoutedEventArgs e)
        {
            VersionManager.RestoreLatest(_projectRoot);
            LoadProjectTree(_projectRoot);
        }

        private async void PreviewUpdateButton_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_latestVersionPath) || !Directory.Exists(_latestVersionPath))
                return;

            string project = Path.Combine(_latestVersionPath, "src", "ASL.CodeEngineering.App");
            try
            {
                await ProcessRunner.RunAsync("dotnet", $"run --project \"{project}\" --no-build", Path.Combine(_latestVersionPath, "src"), "preview", CancellationToken.None);
                if (MessageBox.Show("Apply this update?", "Preview", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                {
                    VersionManager.RestoreLatest(_projectRoot);
                    LoadProjectTree(_projectRoot);
                }
            }
            catch (Exception ex)
            {
                LogError("PreviewUpdate", ex);
            }
        }

        private void LearningEnabledCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(_learningStatePath, "1");
            StartLearningLoop();
        }

        private void LearningEnabledCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(_learningStatePath, "0");
            PauseLearningLoop();
        }

        private async void StartProjectButton_Click(object sender, RoutedEventArgs e)
        {
            if (_projectTask is not null && !_projectTask.IsCompleted)
                return;

            string desc = ProjectDescriptionTextBox.Text;
            string lang = ProjectLanguageTextBox.Text;
            if (string.IsNullOrWhiteSpace(desc) || string.IsNullOrWhiteSpace(lang))
                return;

            string projectsDir = Path.Combine(AppContext.BaseDirectory, "projects");
            _projectCts = new CancellationTokenSource();
            StartProjectButton.IsEnabled = false;
            StopProjectButton.IsEnabled = true;
            try
            {
                _projectTask = ProjectGenerator.GenerateAsync(desc, lang, projectsDir, _projectCts.Token);
                string path = await (Task<string>)_projectTask;
                ResponseTextBox.Text = path;
            }
            catch (OperationCanceledException)
            {
                ResponseTextBox.Text = "Cancelled";
            }
            finally
            {
                StartProjectButton.IsEnabled = true;
                StopProjectButton.IsEnabled = false;
            }
        }

        private void StopProjectButton_Click(object sender, RoutedEventArgs e)
        {
            _projectCts?.Cancel();
        }

        private static void LogError(string operation, Exception ex)
        {
            string logsDir = Environment.GetEnvironmentVariable("LOGS_DIR") ??
                              Path.Combine(AppContext.BaseDirectory, "logs");

            if (!TryWrite(logsDir))
            {
                string fallback = Path.Combine(AppContext.BaseDirectory, "logs");
                if (fallback != logsDir)
                    TryWrite(fallback);
            }

            bool TryWrite(string dir)
            {
                try
                {
                    Directory.CreateDirectory(dir);
                    var file = Path.Combine(dir, $"{operation}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.log");
                    File.WriteAllText(file, ex.ToString());
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }
    }

    public class LearningEntry
    {
        public DateTime Timestamp { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
    }
}
