using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
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
        private readonly ObservableCollection<PluginEntry> _providerEntries = new();
        private readonly ObservableCollection<PluginEntry> _analyzerEntries = new();
        private readonly ObservableCollection<PluginEntry> _runnerEntries = new();
        private readonly ObservableCollection<PluginEntry> _buildTestEntries = new();
        private readonly Dictionary<string, string> _languageMap = new()
        {
            ["English"] = "en",
            ["Azerbaijani"] = "az",
            ["Russian"] = "ru",
            ["Turkish"] = "tr"
        };
        private readonly ObservableCollection<string> _profileNames = new();
        private readonly Dictionary<string, UserProfile> _profiles = new();
        private string _profilesDir = string.Empty;
        private string? _latestVersionPath;
        private CancellationTokenSource? _projectCts;
        private Task? _projectTask;
        private SyncServer? _syncServer;
        private SyncClient? _syncClient;


        public MainWindow()
        {
            InitializeComponent();

            Permissions.Load(AppContext.BaseDirectory);
            var login = new LoginWindow();
            if (login.ShowDialog() != true)
            {
                Close();
                return;
            }
            Permissions.AssignRole(login.UserName, login.SelectedRole);
            Permissions.SetCurrentUser(login.UserName);
            Permissions.Save(AppContext.BaseDirectory);

            LanguageComboBox.ItemsSource = _languageMap.Keys;
            LanguageComboBox.SelectedIndex = 0;
            UpdateLanguage("en");

            LearningGrid.ItemsSource = _learningEntries;
            _learningStatePath = Path.Combine(AppContext.BaseDirectory, "data", "learning_enabled.txt");

            string baseData = Environment.GetEnvironmentVariable("DATA_DIR") ??
                                Path.Combine(AppContext.BaseDirectory, "data");
            _profilesDir = Path.Combine(baseData, "profiles");
            Directory.CreateDirectory(_profilesDir);
            foreach (var file in Directory.GetFiles(_profilesDir, "*.json"))
            {
                try
                {
                    var prof = JsonSerializer.Deserialize<UserProfile>(File.ReadAllText(file));
                    if (prof != null)
                    {
                        _profiles[prof.Name] = prof;
                        _profileNames.Add(prof.Name);
                    }
                }
                catch { }
            }
            if (_profileNames.Count == 0)
            {
                var def = new UserProfile { Name = "Default" };
                _profiles[def.Name] = def;
                _profileNames.Add(def.Name);
            }
            ProfileComboBox.ItemsSource = _profileNames;
            ProfileComboBox.SelectedIndex = 0;

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

            string builtInVersion = typeof(EchoAIProvider).Assembly.GetName().Version?.ToString() ?? string.Empty;

            _providerFactories["Echo"] = () => new EchoAIProvider();
            _providerEntries.Add(new PluginEntry { Name = "Echo", Version = builtInVersion });

            _providerFactories["Reverse"] = () => new ReverseAIProvider();
            _providerEntries.Add(new PluginEntry { Name = "Reverse", Version = builtInVersion });

            _providerFactories["Local"] = () => new LocalAIProvider();
            _providerEntries.Add(new PluginEntry { Name = "Local", Version = builtInVersion });

            _providerFactories["OpenAI"] = () => new OpenAIProvider();
            _providerEntries.Add(new PluginEntry { Name = "OpenAI", Version = builtInVersion });

            _analyzerFactories["Todo"] = () => new TodoAnalyzer();
            _runnerFactories["DotnetVersion"] = () => new DotnetVersionRunner();
            _buildTestRunnerFactories["Dotnet"] = () => new DotnetBuildTestRunner();
            _buildTestRunnerFactories["Python"] = () => new PythonBuildTestRunner();

            var providerVersions = new Dictionary<string, string>();
            foreach (var pair in AIProviderLoader.LoadProviders(AppContext.BaseDirectory, providerVersions))
            {
                if (_providerFactories.ContainsKey(pair.Key))
                {
                    StatusTextBlock.Text = string.Format(Strings.DuplicateProvider, pair.Key);
                    LogError("DuplicateProvider", new InvalidOperationException($"Duplicate provider name '{pair.Key}'"));
                }
                else
                {
                    _providerFactories[pair.Key] = pair.Value;
                    _providerEntries.Add(new PluginEntry { Name = pair.Key, Version = providerVersions.GetValueOrDefault(pair.Key, string.Empty) });
                }
            }

            var analyzerVersions = new Dictionary<string, string>();
            foreach (var pair in PluginLoader.LoadAnalyzers(AppContext.BaseDirectory, analyzerVersions))
            {
                if (_analyzerFactories.ContainsKey(pair.Key))
                {
                    StatusTextBlock.Text = string.Format(Strings.DuplicateAnalyzer, pair.Key);
                    LogError("DuplicateAnalyzer", new InvalidOperationException($"Duplicate analyzer name '{pair.Key}'"));
                }
                else
                {
                    _analyzerFactories[pair.Key] = pair.Value;
                    _analyzerEntries.Add(new PluginEntry { Name = pair.Key, Version = analyzerVersions.GetValueOrDefault(pair.Key, string.Empty) });
                }
            }

            var runnerVersions = new Dictionary<string, string>();
            foreach (var pair in PluginLoader.LoadRunners(AppContext.BaseDirectory, runnerVersions))
            {
                if (_runnerFactories.ContainsKey(pair.Key))
                {
                    StatusTextBlock.Text = string.Format(Strings.DuplicateRunner, pair.Key);
                    LogError("DuplicateRunner", new InvalidOperationException($"Duplicate runner name '{pair.Key}'"));
                }
                else
                {
                    _runnerFactories[pair.Key] = pair.Value;
                    _runnerEntries.Add(new PluginEntry { Name = pair.Key, Version = runnerVersions.GetValueOrDefault(pair.Key, string.Empty) });
                }
            }

            var buildVersions = new Dictionary<string, string>();
            foreach (var pair in PluginLoader.LoadBuildTestRunners(AppContext.BaseDirectory, buildVersions))
            {
                if (_buildTestRunnerFactories.ContainsKey(pair.Key))
                {
                    StatusTextBlock.Text = string.Format(Strings.DuplicateBuildTestRunner, pair.Key);
                    LogError("DuplicateBuildTestRunner", new InvalidOperationException($"Duplicate build/test runner name '{pair.Key}'"));
                }
                else
                {
                    _buildTestRunnerFactories[pair.Key] = pair.Value;
                    _buildTestEntries.Add(new PluginEntry { Name = pair.Key, Version = buildVersions.GetValueOrDefault(pair.Key, string.Empty) });
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
                    {
                        _providerFactories.Remove(key);
                        var entry = _providerEntries.FirstOrDefault(p => p.Name == key);
                        if (entry != null)
                            _providerEntries.Remove(entry);
                    }
                }
            }

            ProviderComboBox.ItemsSource = _providerEntries.Where(p => p.Enabled).Select(p => p.Name);
            ProviderComboBox.SelectedIndex = ProviderComboBox.Items.Count > 0 ? 0 : -1;
            AnalyzerComboBox.ItemsSource = _analyzerEntries.Where(a => a.Enabled).Select(a => a.Name);
            AnalyzerComboBox.SelectedIndex = AnalyzerComboBox.Items.Count > 0 ? 0 : -1;
            RunnerComboBox.ItemsSource = _runnerEntries.Where(r => r.Enabled).Select(r => r.Name);
            RunnerComboBox.SelectedIndex = RunnerComboBox.Items.Count > 0 ? 0 : -1;
            BuildTestRunnerComboBox.ItemsSource = _buildTestEntries.Where(b => b.Enabled).Select(b => b.Name);
            BuildTestRunnerComboBox.SelectedIndex = BuildTestRunnerComboBox.Items.Count > 0 ? 0 : -1;

            if (ProfileComboBox.SelectedItem is string pname &&
                _profiles.TryGetValue(pname, out var prof))
            {
                if (!string.IsNullOrEmpty(prof.Provider))
                    ProviderComboBox.SelectedItem = prof.Provider;
                if (!string.IsNullOrEmpty(prof.Analyzer))
                    AnalyzerComboBox.SelectedItem = prof.Analyzer;
                if (!string.IsNullOrEmpty(prof.Runner))
                    RunnerComboBox.SelectedItem = prof.Runner;
                if (!string.IsNullOrEmpty(prof.BuildTestRunner))
                    BuildTestRunnerComboBox.SelectedItem = prof.BuildTestRunner;
                if (!string.IsNullOrEmpty(prof.LastProject) && Directory.Exists(prof.LastProject))
                    _projectRoot = prof.LastProject;
            }

            ProviderGrid.ItemsSource = _providerEntries;
            AnalyzerGrid.ItemsSource = _analyzerEntries;
            RunnerGrid.ItemsSource = _runnerEntries;
            BuildTestGrid.ItemsSource = _buildTestEntries;

            LoadProjectTree(_projectRoot);

            if (File.Exists(_learningStatePath) && File.ReadAllText(_learningStatePath).Trim() == "1")
            {
                LearningEnabledCheckBox.IsChecked = true;
                StartLearningLoop();
            }

            if (Environment.GetEnvironmentVariable("ENABLE_SYNC_SERVER") == "1")
            {
                _syncServer = new SyncServer(6001);
                try { _syncServer.StartAsync().Wait(); } catch { }
            }

            string url = Environment.GetEnvironmentVariable("SYNC_SERVER_URL");
            if (!string.IsNullOrEmpty(url))
            {
                _syncClient = new SyncClient(url, AppContext.BaseDirectory);
                try { _syncClient.StartAsync().Wait(); } catch { }
            }
        }

        private void ProviderComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProviderComboBox.SelectedItem is string key &&
                _providerFactories.TryGetValue(key, out var factory))
            {
                _aiProvider = factory();
                SaveCurrentProfile();
            }
        }

        private void AnalyzerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (AnalyzerComboBox.SelectedItem is string key &&
                _analyzerFactories.TryGetValue(key, out var factory))
            {
                _analyzer = factory();
                SaveCurrentProfile();
            }
        }

        private void RunnerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (RunnerComboBox.SelectedItem is string key &&
                _runnerFactories.TryGetValue(key, out var factory))
            {
                _runner = factory();
                SaveCurrentProfile();
            }
        }

        private void BuildTestRunnerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (BuildTestRunnerComboBox.SelectedItem is string key &&
                _buildTestRunnerFactories.TryGetValue(key, out var factory))
            {
                _buildTestRunner = factory();
                SaveCurrentProfile();
            }
        }

        private void LanguageComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LanguageComboBox.SelectedItem is string name &&
                _languageMap.TryGetValue(name, out var code))
            {
                UpdateLanguage(code);
            }
        }

        private void ProfileComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ProfileComboBox.SelectedItem is string name &&
                _profiles.TryGetValue(name, out var prof))
            {
                if (!string.IsNullOrEmpty(prof.Provider))
                    ProviderComboBox.SelectedItem = prof.Provider;
                if (!string.IsNullOrEmpty(prof.Analyzer))
                    AnalyzerComboBox.SelectedItem = prof.Analyzer;
                if (!string.IsNullOrEmpty(prof.Runner))
                    RunnerComboBox.SelectedItem = prof.Runner;
                if (!string.IsNullOrEmpty(prof.BuildTestRunner))
                    BuildTestRunnerComboBox.SelectedItem = prof.BuildTestRunner;
                if (!string.IsNullOrEmpty(prof.LastProject) && Directory.Exists(prof.LastProject))
                {
                    _projectRoot = prof.LastProject;
                    LoadProjectTree(_projectRoot);
                }
            }
        }


        private async void SendButton_Click(object sender, RoutedEventArgs e)
        {
            string prompt = PromptTextBox.Text;
            if (string.IsNullOrWhiteSpace(prompt))
                return;

            SendButton.IsEnabled = false;
            StatusTextBlock.Text = Strings.Sending;

            string response;
            try
            {
                response = await _aiProvider.SendChatAsync(prompt, CancellationToken.None);
                ResponseTextBox.Text = response;
                StatusTextBlock.Text = Strings.Done;
            }
            catch (Exception ex)
            {
                response = ex.Message;
                ResponseTextBox.Text = ex.Message;
                StatusTextBlock.Text = Strings.Error;
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
                StatusTextBlock.Text = Strings.LogError;
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
                StatusTextBlock.Text = Strings.SummaryError;
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
                StatusTextBlock.Text = Strings.LogError;
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
            StatusTextBlock.Text = Strings.Analyzing;
            try
            {
                var result = await _analyzer.AnalyzeAsync(code, CancellationToken.None);
                ResponseTextBox.Text = result;
                StatusTextBlock.Text = Strings.Done;
            }
            catch (Exception ex)
            {
                ResponseTextBox.Text = ex.Message;
                StatusTextBlock.Text = Strings.Error;
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
            StatusTextBlock.Text = Strings.Running;
            try
            {
                var result = await _runner.RunAsync(_projectRoot, CancellationToken.None);
                ResponseTextBox.Text = result;
                StatusTextBlock.Text = Strings.Done;
            }
            catch (Exception ex)
            {
                ResponseTextBox.Text = ex.Message;
                StatusTextBlock.Text = Strings.Error;
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
            StatusTextBlock.Text = Strings.Building;
            try
            {
                _latestVersionPath = await BuildProcess.BuildNewVersionAsync(_projectRoot, _buildTestRunner, CancellationToken.None);
                ResponseTextBox.Text = _latestVersionPath;
                StatusTextBlock.Text = Strings.Done;
            }
            catch (Exception ex)
            {
                ResponseTextBox.Text = ex.Message;
                StatusTextBlock.Text = Strings.Error;
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
            StatusTextBlock.Text = Strings.Testing;
            try
            {
                var result = await _buildTestRunner.TestAsync(_projectRoot, CancellationToken.None);
                ResponseTextBox.Text = result;
                StatusTextBlock.Text = Strings.Done;
            }
            catch (Exception ex)
            {
                ResponseTextBox.Text = ex.Message;
                StatusTextBlock.Text = Strings.Error;
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
                SaveCurrentProfile();
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
            StatusTextBlock.Text = Strings.Learning;
        }

        private void PauseLearningLoop()
        {
            _learningCts?.Cancel();
            _monitorCts?.Cancel();
            StatusTextBlock.Text = Strings.Paused;
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

        private void ProviderGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            ProviderComboBox.ItemsSource = _providerEntries.Where(p => p.Enabled).Select(p => p.Name);
            if (ProviderComboBox.SelectedItem == null && ProviderComboBox.Items.Count > 0)
                ProviderComboBox.SelectedIndex = 0;
        }

        private void AnalyzerGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            AnalyzerComboBox.ItemsSource = _analyzerEntries.Where(a => a.Enabled).Select(a => a.Name);
            if (AnalyzerComboBox.SelectedItem == null && AnalyzerComboBox.Items.Count > 0)
                AnalyzerComboBox.SelectedIndex = 0;
        }

        private void RunnerGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            RunnerComboBox.ItemsSource = _runnerEntries.Where(r => r.Enabled).Select(r => r.Name);
            if (RunnerComboBox.SelectedItem == null && RunnerComboBox.Items.Count > 0)
                RunnerComboBox.SelectedIndex = 0;
        }

        private void BuildTestGrid_CellEditEnding(object sender, DataGridCellEditEndingEventArgs e)
        {
            BuildTestRunnerComboBox.ItemsSource = _buildTestEntries.Where(b => b.Enabled).Select(b => b.Name);
            if (BuildTestRunnerComboBox.SelectedItem == null && BuildTestRunnerComboBox.Items.Count > 0)
                BuildTestRunnerComboBox.SelectedIndex = 0;
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
                if (MessageBox.Show(Strings.ApplyUpdateQuestion, Strings.PreviewCaption, MessageBoxButton.YesNo) == MessageBoxResult.Yes)
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
                ResponseTextBox.Text = Strings.Cancelled;
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

        private void UpdateLanguage(string code)
        {
            Strings.Culture = new CultureInfo(code);
            Title = Strings.AppTitle;
            OpenProjectButton.Content = Strings.OpenProject;
            AnalyzeButton.Content = Strings.Analyze;
            RunButton.Content = Strings.Run;
            BuildButton.Content = Strings.Build;
            TestButton.Content = Strings.Test;
            PreviewUpdateButton.Content = Strings.PreviewUpdate;
            SendButton.Content = Strings.Send;
            StartLearningButton.Content = Strings.StartLearning;
            PauseLearningButton.Content = Strings.Pause;
            ResumeLearningButton.Content = Strings.Resume;
            DashboardButton.Content = Strings.Dashboard;
            AcceptSuggestionButton.Content = Strings.Accept;
            RollbackSuggestionButton.Content = Strings.Rollback;
            LearningEnabledCheckBox.Content = Strings.LearningOn;
            StartProjectButton.Content = Strings.StartNewProject;
            StopProjectButton.Content = Strings.Stop;
            MainTab.Header = Strings.MainTab;
            PluginsTab.Header = Strings.PluginsTab;
            ProvidersTextBlock.Text = Strings.ProvidersHeader;
            AnalyzersTextBlock.Text = Strings.AnalyzersHeader;
            RunnersTextBlock.Text = Strings.RunnersHeader;
            BuildTestTextBlock.Text = Strings.BuildTestRunnersHeader;
            TimeColumn.Header = Strings.TimeHeader;
            ProviderColumn.Header = Strings.ProviderHeader;
            SuggestionColumn.Header = Strings.SuggestionHeader;
            ProviderEnabledColumn.Header = Strings.EnabledColumn;
            ProviderNameColumn.Header = Strings.NameColumn;
            ProviderVersionColumn.Header = Strings.VersionColumn;
            AnalyzerEnabledColumn.Header = Strings.EnabledColumn;
            AnalyzerNameColumn.Header = Strings.NameColumn;
            AnalyzerVersionColumn.Header = Strings.VersionColumn;
            RunnerEnabledColumn.Header = Strings.EnabledColumn;
            RunnerNameColumn.Header = Strings.NameColumn;
            RunnerVersionColumn.Header = Strings.VersionColumn;
            BuildEnabledColumn.Header = Strings.EnabledColumn;
            BuildNameColumn.Header = Strings.NameColumn;
            BuildVersionColumn.Header = Strings.VersionColumn;
        }

        private void SaveCurrentProfile()
        {
            if (ProfileComboBox.SelectedItem is not string name ||
                !_profiles.TryGetValue(name, out var prof))
                return;

            prof.Provider = ProviderComboBox.SelectedItem as string;
            prof.Analyzer = AnalyzerComboBox.SelectedItem as string;
            prof.Runner = RunnerComboBox.SelectedItem as string;
            prof.BuildTestRunner = BuildTestRunnerComboBox.SelectedItem as string;
            prof.LastProject = _projectRoot;
            if (!string.IsNullOrEmpty(_projectRoot))
            {
                if (!prof.RecentProjects.Contains(_projectRoot))
                    prof.RecentProjects.Add(_projectRoot);
                while (prof.RecentProjects.Count > 5)
                    prof.RecentProjects.RemoveAt(0);
            }

            string path = Path.Combine(_profilesDir, $"{name}.json");
            File.WriteAllText(path, JsonSerializer.Serialize(prof, new JsonSerializerOptions
            {
                WriteIndented = true
            }));
        }

        private static void LogError(string operation, Exception ex)
        {
            try
            {
                SecureLogger.Write(operation, ex.ToString());
            }
            catch
            {
                // ignore logging failures
            }
        }

        protected override void OnClosed(EventArgs e)
        {
            _syncClient?.DisposeAsync().AsTask().Wait();
            _syncServer?.DisposeAsync().AsTask().Wait();
            base.OnClosed(e);
        }
    }

    public class LearningEntry
    {
        public DateTime Timestamp { get; set; }
        public string Provider { get; set; } = string.Empty;
        public string Result { get; set; } = string.Empty;
    }

    public class PluginEntry
    {
        public string Name { get; set; } = string.Empty;
        public string Version { get; set; } = string.Empty;
        public bool Enabled { get; set; } = true;
    }
}
