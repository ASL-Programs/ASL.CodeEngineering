using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading;
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


        public MainWindow()
        {
            InitializeComponent();

            _providerFactories["Echo"] = () => new EchoAIProvider();
            _providerFactories["OpenAI"] = () => new OpenAIProvider();

            _analyzerFactories["Todo"] = () => new TodoAnalyzer();
            _runnerFactories["DotnetVersion"] = () => new DotnetVersionRunner();
            _buildTestRunnerFactories["Dotnet"] = () => new DotnetBuildTestRunner();
            _buildTestRunnerFactories["Python"] = () => new PythonBuildTestRunner();

            foreach (var pair in AIProviderLoader.LoadProviders(AppContext.BaseDirectory))
            {
                if (!_providerFactories.ContainsKey(pair.Key))
                    _providerFactories[pair.Key] = pair.Value;
            }

            foreach (var pair in PluginLoader.LoadAnalyzers(AppContext.BaseDirectory))
            {
                if (!_analyzerFactories.ContainsKey(pair.Key))
                    _analyzerFactories[pair.Key] = pair.Value;
            }

            foreach (var pair in PluginLoader.LoadRunners(AppContext.BaseDirectory))
            {
                if (!_runnerFactories.ContainsKey(pair.Key))
                    _runnerFactories[pair.Key] = pair.Value;
            }

            foreach (var pair in PluginLoader.LoadBuildTestRunners(AppContext.BaseDirectory))
            {
                if (!_buildTestRunnerFactories.ContainsKey(pair.Key))
                    _buildTestRunnerFactories[pair.Key] = pair.Value;
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
            }
            finally
            {
                SendButton.IsEnabled = true;

                string providerName = _aiProvider.Name;
                string dir = Path.Combine(AppContext.BaseDirectory, "data", providerName);
                Directory.CreateDirectory(dir);
                string path = Path.Combine(dir, "chatlog.jsonl");
                var entry = new { timestamp = DateTime.UtcNow, prompt, response };
                string line = JsonSerializer.Serialize(entry);
                File.AppendAllText(path, line + Environment.NewLine);
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
                var result = await _runner.RunAsync(AppContext.BaseDirectory, CancellationToken.None);
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
                var result = await _buildTestRunner.BuildAsync(_projectRoot, CancellationToken.None);
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
            foreach (var dir in Directory.GetDirectories(path))
            {
                var item = new TreeViewItem { Header = Path.GetFileName(dir), Tag = dir };
                LoadDirectory(item, dir);
                parentItem.Items.Add(item);
            }

            foreach (var file in Directory.GetFiles(path))
            {
                parentItem.Items.Add(new TreeViewItem { Header = Path.GetFileName(file), Tag = file });
            }
        }
    }
}
